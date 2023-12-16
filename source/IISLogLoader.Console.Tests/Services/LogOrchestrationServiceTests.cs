using IISLogLoader.Common.Data;
using IISLogLoader.Common.IO;
using IISLogLoader.Console.Factories;
using IISLogLoader.Console.Models;
using IISLogLoader.Console.Repositories;
using IISLogLoader.Console.Services;
using IISLogLoader.Test.Common;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Tests.Services
{
    [TestFixture]
    public class LogOrchestrationServiceTests
    {
        [Test]
        public void ProcessLogs_LoadsJournal()
        {
            // setup 
            LogStorageInfo logStorageInfo = new SubstituteBuilder<LogStorageInfo>().WithRandomProperties().Build();
            IJournalRepository journalRepository = new SubstituteBuilder<IJournalRepository>().Build();

            // execute
            ILogOrchestrationService orchestrator = CreateOrchestrationService(journalRepository: journalRepository);
            orchestrator.ProcessLogs(logStorageInfo).GetAwaiter().GetResult();

            // assert
            journalRepository.Received(1).Load(logStorageInfo.Name);
        }

        [Test]
        public void ProcessLogs_RunsDbMigrations()
        {
            // setup 
            LogStorageInfo logStorageInfo = new SubstituteBuilder<LogStorageInfo>().WithRandomProperties().Build();
            ILogDbContext logDbContext = new SubstituteBuilder<ILogDbContext>().Build();
            ILogDbContextFactory logDbContextFactory = new SubstituteBuilder<ILogDbContextFactory>().Build();
            logDbContextFactory.GetDbContext(logStorageInfo.DbType, logStorageInfo.ConnectionString, logStorageInfo.TableName).Returns(logDbContext);

            // execute
            ILogOrchestrationService orchestrator = CreateOrchestrationService(logDbContextFactory: logDbContextFactory);
            orchestrator.ProcessLogs(logStorageInfo).GetAwaiter().GetResult();

            // assert
            logDbContextFactory.Received(1).GetDbContext(logStorageInfo.DbType, logStorageInfo.ConnectionString, logStorageInfo.TableName);
            logDbContext.Received(1).RunMigrations(logStorageInfo.TableName);
        }

        [Test]
        public void ProcessLogs_NoWatchFolders_DoesNotLoadModifiedFiles()
        {
            // setup
            LogStorageInfo logStorageInfo = new SubstituteBuilder<LogStorageInfo>().Build();
            IFileLoadService fileLoadService = new SubstituteBuilder<IFileLoadService>().Build();

            // execute
            ILogOrchestrationService orchestrator = CreateOrchestrationService(fileLoadService: fileLoadService);
            orchestrator.ProcessLogs(logStorageInfo).GetAwaiter().GetResult();

            // assert
            fileLoadService.DidNotReceive().FindModifiedFiles(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Journal>());

        }

        [Test]
        public void ProcessLogs_WatchFoldersConfigured_LoadsModifiedFilesForEach()
        {
            // setup
            string[] watchFolders = { "folder1", "folder2", "folder3" };
            LogStorageInfo logStorageInfo = new SubstituteBuilder<LogStorageInfo>()
                .WithProperty(x => x.FileSearchPattern, RandomData.String.Letters(3, 10))
                .WithProperty(x => x.WatchFolders, watchFolders)
                .Build();
            IFileLoadService fileLoadService = new SubstituteBuilder<IFileLoadService>().Build();

            IJournalRepository journalRepository = new SubstituteBuilder<IJournalRepository>().Build();
            Journal journal = new SubstituteBuilder<Journal>().WithRandomProperties().Build();
            journalRepository.Load(Arg.Any<string>()).Returns(journal);

            // execute
            ILogOrchestrationService orchestrator = CreateOrchestrationService(journalRepository: journalRepository, fileLoadService: fileLoadService);
            orchestrator.ProcessLogs(logStorageInfo).GetAwaiter().GetResult();

            // assert
            fileLoadService.Received(3).FindModifiedFiles(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Journal>());
            fileLoadService.Received(1).FindModifiedFiles(watchFolders[0], logStorageInfo.FileSearchPattern, journal);
            fileLoadService.Received(1).FindModifiedFiles(watchFolders[1], logStorageInfo.FileSearchPattern, journal);
            fileLoadService.Received(1).FindModifiedFiles(watchFolders[2], logStorageInfo.FileSearchPattern, journal);
        }

        [Test]
        public void ProcessLogs_NoModifiedFiles_NoImportOccurs()
        {
            // setup
            string[] watchFolders = { "folder1" };
            LogStorageInfo logStorageInfo = new SubstituteBuilder<LogStorageInfo>()
                .WithRandomProperties()
                .WithProperty(x => x.WatchFolders, watchFolders)
                .Build();

            IJournalRepository journalRepository = new SubstituteBuilder<IJournalRepository>().Build();
            Journal journal = new SubstituteBuilder<Journal>().WithRandomProperties().Build();
            journalRepository.Load(Arg.Any<string>()).Returns(journal);

            IFileLoadService fileLoadService = new SubstituteBuilder<IFileLoadService>().Build();
            fileLoadService.FindModifiedFiles(watchFolders[0], logStorageInfo.FileSearchPattern, journal).Returns(Enumerable.Empty<FileInfoWrapper>());

            // execute
            ILogOrchestrationService orchestrator = CreateOrchestrationService(journalRepository: journalRepository, fileLoadService: fileLoadService);
            orchestrator.ProcessLogs(logStorageInfo).GetAwaiter().GetResult();

            // assert
            fileLoadService.Received(1).FindModifiedFiles(watchFolders[0], logStorageInfo.FileSearchPattern, journal);
            journal.DidNotReceive().Update(Arg.Any<FileInfoWrapper>());
        }

        [Test]
        public void ProcessLogs_ModifiedFilesExist_ImportOccursInTransaction()
        {
            // setup
            string[] watchFolders = { "folder1" };
            LogStorageInfo logStorageInfo = new SubstituteBuilder<LogStorageInfo>()
                .WithRandomProperties()
                .WithProperty(x => x.WatchFolders, watchFolders)
                .Build();

            IJournalRepository journalRepository = new SubstituteBuilder<IJournalRepository>().Build();
            Journal journal = new SubstituteBuilder<Journal>().WithRandomProperties().Build();
            journalRepository.Load(Arg.Any<string>()).Returns(journal);

            List<FileInfoWrapper> modifiedFiles = CreateModifiedFiles();
            IFileLoadService fileLoadService = new SubstituteBuilder<IFileLoadService>().Build();
            fileLoadService.FindModifiedFiles(watchFolders[0], logStorageInfo.FileSearchPattern, journal).Returns(modifiedFiles);

            ILogDbContext logDbContext = new SubstituteBuilder<ILogDbContext>().Build();
            ILogDbContextFactory logDbContextFactory = new SubstituteBuilder<ILogDbContextFactory>().Build();
            logDbContextFactory.GetDbContext(logStorageInfo.DbType, logStorageInfo.ConnectionString, logStorageInfo.TableName).Returns(logDbContext);

            IFileImportService fileImportService = new SubstituteBuilder<IFileImportService>().Build(); 

            // execute
            ILogOrchestrationService orchestrator = CreateOrchestrationService(journalRepository: journalRepository, logDbContextFactory: logDbContextFactory, fileLoadService: fileLoadService, fileImportService: fileImportService);
            orchestrator.ProcessLogs(logStorageInfo).GetAwaiter().GetResult();

            // assert
            logDbContext.Received(modifiedFiles.Count).BeginTransaction();
            fileImportService.Received(modifiedFiles.Count).ImportLogFile(logDbContext, Arg.Any<string>());
            logDbContext.Received(modifiedFiles.Count).Commit();
            foreach (FileInfoWrapper fiw in modifiedFiles)
            {
                fileImportService.Received(1).ImportLogFile(logDbContext, fiw.FullName);
            }
        }

        [Test]
        public void ProcessLogs_ModifiedFilesExist_JournalUpdated()
        {
            // setup
            string[] watchFolders = { "folder1" };
            LogStorageInfo logStorageInfo = new SubstituteBuilder<LogStorageInfo>()
                .WithRandomProperties()
                .WithProperty(x => x.WatchFolders, watchFolders)
                .Build();

            IJournalRepository journalRepository = new SubstituteBuilder<IJournalRepository>().Build();
            Journal journal = new SubstituteBuilder<Journal>().WithRandomProperties().Build();
            journalRepository.Load(Arg.Any<string>()).Returns(journal);

            List<FileInfoWrapper> modifiedFiles = CreateModifiedFiles();
            IFileLoadService fileLoadService = new SubstituteBuilder<IFileLoadService>().Build();
            fileLoadService.FindModifiedFiles(watchFolders[0], logStorageInfo.FileSearchPattern, journal).Returns(modifiedFiles);

            // execute
            ILogOrchestrationService orchestrator = CreateOrchestrationService(journalRepository: journalRepository, fileLoadService: fileLoadService);
            orchestrator.ProcessLogs(logStorageInfo).GetAwaiter().GetResult();

            // assert
            journal.Received(modifiedFiles.Count).Update(Arg.Any<FileInfoWrapper>());
            journalRepository.Received(modifiedFiles.Count).Save(journal);
            foreach (FileInfoWrapper fiw in modifiedFiles)
            {
                journal.Received(1).Update(fiw);
            }
        }

        private List<FileInfoWrapper> CreateModifiedFiles()
        {
            var files = new List<FileInfoWrapper>();
            int count = RandomData.Number.Next(2, 7);
            for (int i=0; i<count; i++)
            {
                files.Add(new SubstituteBuilder<FileInfoWrapper>().WithRandomProperties().Build());
            }
            return files;
        }

        private ILogOrchestrationService CreateOrchestrationService(
            IJournalRepository? journalRepository = null
            , ILogDbContextFactory? logDbContextFactory = null
            , IFileLoadService? fileLoadService = null
            , IFileImportService? fileImportService = null
            )
        {
            ILogger<LogOrchestrationService> logger = new SubstituteBuilder<ILogger<LogOrchestrationService>>().Build();
            journalRepository ??= new SubstituteBuilder<IJournalRepository>().Build();
            logDbContextFactory ??= new SubstituteBuilder<ILogDbContextFactory>().Build();
            fileLoadService ??= new SubstituteBuilder<IFileLoadService>().Build();
            fileImportService ??= new SubstituteBuilder<IFileImportService>().Build();
            return new LogOrchestrationService(logger, journalRepository, logDbContextFactory, fileLoadService, fileImportService);
        }


    }
}
