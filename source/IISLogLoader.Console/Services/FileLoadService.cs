using IISLogLoader.Common.IO;
using IISLogLoader.Common.Models;
using IISLogLoader.Common.Models.Mappers;
using IISLogLoader.Console.Models;
using IISLogParser;
using Microsoft.Extensions.Logging;
using Tx.Windows;

namespace IISLogLoader.Console.Services
{
    public interface IFileLoadService
    {
        Task<IEnumerable<FileInfoWrapper>> FindModifiedFiles(string folder, string fileSearchPattern, Journal journal);

        LogFileLoadResult LoadFile(string filePath);
    }

    public class FileLoadService : IFileLoadService
    {
        private readonly ILogger<FileLoadService> _logger;
        private readonly IDirectoryWrapper _directoryUtility;
        private readonly IFileWrapper _fileWrapper;

        public FileLoadService(ILogger<FileLoadService> logger, IDirectoryWrapper directoryUtility, IFileWrapper fileWrapper)
        {
            _logger = logger;
            _directoryUtility = directoryUtility;
            _fileWrapper = fileWrapper;
        }

        public async Task<IEnumerable<FileInfoWrapper>> FindModifiedFiles(string folder, string fileSearchPattern, Journal journal)
        {
            var filesOnDisk = _directoryUtility.EnumerateFiles(folder, fileSearchPattern);
            List<FileInfoWrapper> modifiedFiles = new List<FileInfoWrapper>();

            foreach (string file in filesOnDisk) 
            {
                JournalEntry? journalEntry = journal.GetEntry(file);
                FileInfoWrapper fi = _fileWrapper.GetFileInfo(file);
                if (journalEntry == null || journalEntry.Length != fi.Length || journalEntry.LastModified != fi.LastWriteTime)
                {
                    modifiedFiles.Add(fi);
                }
            }

            return await Task.FromResult(modifiedFiles);
        }

        public LogFileLoadResult LoadFile(string filePath)
        {
            LogFileLoadResult result = new LogFileLoadResult();

            try
            {
                var events = W3CEnumerable.FromFile(filePath);
                events.Count();     // must be called to force the enumeration check - throws an error for invalid log files
                result.LogEvents = LogEventMapper.MapFromW3CEvents(filePath, events);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("File load failed using Tx.Windows.W3CEnumerable: {message}", ex.Message);
                result.ErrorMessage = ex.Message;
                result.Success = false;
            }

            if (!result.Success)
            {
                try
                {
                    List<IISLogEvent> logs = new List<IISLogEvent>();
                    using (ParserEngine parser = new ParserEngine(filePath))
                    {
                        while (parser.MissingRecords)
                        {
                            logs = parser.ParseLog().ToList();
                        }
                    }
                    result.LogEvents = LogEventMapper.MapFromIISLogEvents(filePath, logs);
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("File load failed using IISLogParser.ParserEngine: {message}", ex.Message);
                    result.ErrorMessage = ex.Message;
                    result.Success = false;
                }
            }

            return result;
        }


    }
}
