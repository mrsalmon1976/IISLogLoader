using IISLogLoader.Common.Data;
using IISLogLoader.Common.IO;
using IISLogLoader.Common.Models;
using IISLogLoader.Console.Factories;
using IISLogLoader.Console.Models;
using IISLogLoader.Console.Repositories;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IISLogLoader.Console.Services
{
    public interface ILogOrchestrationService
    {
        Task ProcessLogs(LogStorageInfo logStorageInfo);
    }

    public class LogOrchestrationService : ILogOrchestrationService
    {
        private readonly ILogger<LogOrchestrationService> _logger;
        private readonly IJournalRepository _journalRepository;
        private readonly ILogDbContextFactory _logDbContextFactory;
        private readonly IFileLoadService _fileLoadService;
        private readonly IFileImportService _fileImportService;

        public LogOrchestrationService(ILogger<LogOrchestrationService> logger
            , IJournalRepository journalRepository
            , ILogDbContextFactory logDbContextFactory
            , IFileLoadService fileLoadService
            , IFileImportService fileImportService)
        {
            _logger = logger;
            _journalRepository = journalRepository;
            _logDbContextFactory = logDbContextFactory;
            _fileLoadService = fileLoadService;
            _fileImportService = fileImportService;
        }

        public async Task ProcessLogs(LogStorageInfo logStorageInfo)
        {
            try
            {
                // load the journal file
                Journal journal = _journalRepository.Load(logStorageInfo.Name);

                _logger.LogDebug("Creating database connection, database type {dbType}, table {tableName}", logStorageInfo.DbType, logStorageInfo.TableName);
                using ILogDbContext dbContext = _logDbContextFactory.GetDbContext(logStorageInfo.DbType, logStorageInfo.ConnectionString, logStorageInfo.TableName);
                
                _logger.LogDebug("Running checks on table existence");
                await dbContext.RunMigrations(logStorageInfo.TableName);

                foreach (string folder in logStorageInfo.WatchFolders)
                {
                    // load files that have changed
                    IEnumerable<FileInfoWrapper> modifiedFiles = await _fileLoadService.FindModifiedFiles(folder, logStorageInfo.FileSearchPattern, journal);
                    _logger.LogInformation("{fileCount} new/modified files found for import", modifiedFiles.Count());

                    foreach (FileInfoWrapper fileInfo in modifiedFiles)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        _logger.LogInformation("Loading file {fileName} [{filePath}]", fileInfo.Name, fileInfo.FullName);
                        dbContext.BeginTransaction();
                        LogFileLoadResult loadResult = await _fileImportService.ImportLogFile(dbContext, fileInfo.FullName);
                        journal.Update(fileInfo, loadResult.Success, loadResult.ErrorMessage);
                        _journalRepository.Save(journal);
                        dbContext.Commit();
                        _logger.LogInformation("{loadResult} load of file {fileName} in {seconds} seconds [{filePath}]"
                            , (loadResult.Success ? "Successful" : "FAILED")
                            , fileInfo.Name
                            , stopwatch.Elapsed.TotalSeconds
                            , fileInfo.FullName);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
