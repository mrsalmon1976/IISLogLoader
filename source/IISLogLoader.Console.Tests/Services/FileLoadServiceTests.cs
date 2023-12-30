using IISLogLoader.Common.IO;
using IISLogLoader.Common.Models;
using IISLogLoader.Console.Models;
using IISLogLoader.Console.Services;
using IISLogLoader.Test.Common;
using IISLogLoader.Test.Common.TestAssets;
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
    public class FileLoadServiceTests
    {
        private static string TestFolder = Path.Combine(AppContext.BaseDirectory, "FileLoadServiceTests");

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(TestFolder);
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(TestFolder, true);
        }

        [Test]
        public void FindModifiedFiles_EnumeratesFilesWithSearchPattern()
        {
            // set up
            string folder = Path.Combine(TestFolder, RandomData.String.Letters(3, 10));

            string searchPattern = $"*.{RandomData.String.Letters(2, 5)}".ToLower();
            Journal journal = new SubstituteBuilder<Journal>().Build();
            IDirectoryWrapper dirWrapper = new SubstituteBuilder<IDirectoryWrapper>().Build();

            // execute
            IFileLoadService fileLoadService = CreateService(dirWrapper: dirWrapper);
            fileLoadService.FindModifiedFiles(folder, searchPattern, journal);

            // assert
            dirWrapper.Received(1).EnumerateFiles(folder, searchPattern);

        }

        [Test]
        public void FindModifiedFiles_NoFiles_ReturnsNoFiles()
        {
            // set up
            string folder = Path.Combine(TestFolder, RandomData.String.Letters(3, 10));

            string searchPattern = $"*.{RandomData.String.Letters(2, 5)}".ToLower();
            Journal journal = new SubstituteBuilder<Journal>().Build();

            IDirectoryWrapper dirWrapper = new SubstituteBuilder<IDirectoryWrapper>().Build();
            dirWrapper.EnumerateFiles(folder, searchPattern).Returns(Enumerable.Empty<string>());

            // execute
            IFileLoadService fileLoadService = CreateService(dirWrapper: dirWrapper);
            var result = fileLoadService.FindModifiedFiles(folder, searchPattern, journal).Result;

            // assert
            journal.DidNotReceive().GetEntry(Arg.Any<string>());
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void FindModifiedFiles_NoModifiiedFiles_ReturnsNoFiles()
        {
            // set up
            string folder = Path.Combine(TestFolder, RandomData.String.Letters(3, 10));

            string searchPattern = $"*.{RandomData.String.Letters(2, 5)}".ToLower();

            string[] filesInFolder = { "file1.txt", "file2.txt", "file3.txt" };
            IDirectoryWrapper dirWrapper = new SubstituteBuilder<IDirectoryWrapper>().Build();
            dirWrapper.EnumerateFiles(folder, searchPattern).Returns(filesInFolder);

            Journal journal = new SubstituteBuilder<Journal>().Build();
            JournalEntry journalEntry = new SubstituteBuilder<JournalEntry>().WithProperty(x => x.Length, 100).Build();
            journal.GetEntry(Arg.Any<string>()).Returns(journalEntry);

            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();
            FileInfoWrapper fileInfoWrapper = new SubstituteBuilder<FileInfoWrapper>().WithProperty(x => x.Length, 100).Build();
            fileWrapper.GetFileInfo(Arg.Any<string>()).Returns(fileInfoWrapper);

            // execute
            IFileLoadService fileLoadService = CreateService(dirWrapper: dirWrapper, fileWrapper: fileWrapper);
            var result = fileLoadService.FindModifiedFiles(folder, searchPattern, journal).Result;

            // assert
            journal.Received(filesInFolder.Length).GetEntry(Arg.Any<string>());
            fileWrapper.Received(filesInFolder.Length).GetFileInfo(Arg.Any<string>());
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void FindModifiedFiles_OnlyModifedFiles_ReturnsAllFiles()
        {
            // set up
            string folder = Path.Combine(TestFolder, RandomData.String.Letters(3, 10));

            string searchPattern = $"*.{RandomData.String.Letters(2, 5)}".ToLower();

            string[] filesInFolder = { "file1.txt", "file2.txt", "file3.txt" };
            IDirectoryWrapper dirWrapper = new SubstituteBuilder<IDirectoryWrapper>().Build();
            dirWrapper.EnumerateFiles(folder, searchPattern).Returns(filesInFolder);

            Journal journal = new SubstituteBuilder<Journal>().Build();
            JournalEntry journalEntry = new SubstituteBuilder<JournalEntry>().WithProperty(x => x.Length, 10).Build();
            journal.GetEntry(Arg.Any<string>()).Returns(journalEntry);

            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();
            FileInfoWrapper fileInfoWrapper = new SubstituteBuilder<FileInfoWrapper>().WithProperty(x => x.Length, 100).Build();
            fileWrapper.GetFileInfo(Arg.Any<string>()).Returns(fileInfoWrapper);

            // execute
            IFileLoadService fileLoadService = CreateService(dirWrapper: dirWrapper, fileWrapper: fileWrapper);
            var result = fileLoadService.FindModifiedFiles(folder, searchPattern, journal).Result;

            // assert
            journal.Received(filesInFolder.Length).GetEntry(Arg.Any<string>());
            fileWrapper.Received(filesInFolder.Length).GetFileInfo(Arg.Any<string>());
            Assert.That(result.Count(), Is.EqualTo(filesInFolder.Length));
        }

        [Test]
        public void FindModifiedFiles_SomeModifedFiles_ReturnsOnlyModifiedFiles()
        {
            // set up
            string folder = Path.Combine(TestFolder, RandomData.String.Letters(3, 10));

            string searchPattern = $"*.{RandomData.String.Letters(2, 5)}".ToLower();

            string[] filesInFolder = { "file1.txt", "file2.txt", "file3.txt" };
            IDirectoryWrapper dirWrapper = new SubstituteBuilder<IDirectoryWrapper>().Build();
            dirWrapper.EnumerateFiles(folder, searchPattern).Returns(filesInFolder);

            Journal journal = new SubstituteBuilder<Journal>().Build();
            journal.GetEntry("file1.txt").Returns(new SubstituteBuilder<JournalEntry>().WithProperty(x => x.Length, 100).Build());
            journal.GetEntry("file2.txt").Returns(new SubstituteBuilder<JournalEntry>().WithProperty(x => x.Length, 99).Build());
            journal.GetEntry("file3.txt").Returns(new SubstituteBuilder<JournalEntry>().WithProperty(x => x.Length, 100).Build());

            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();
            FileInfoWrapper fileInfoWrapper = new SubstituteBuilder<FileInfoWrapper>().WithProperty(x => x.Length, 100).Build();
            fileWrapper.GetFileInfo(Arg.Any<string>()).Returns(fileInfoWrapper);

            // execute
            IFileLoadService fileLoadService = CreateService(dirWrapper: dirWrapper, fileWrapper: fileWrapper);
            var result = fileLoadService.FindModifiedFiles(folder, searchPattern, journal).Result;

            // assert
            journal.Received(filesInFolder.Length).GetEntry(Arg.Any<string>());
            fileWrapper.Received(filesInFolder.Length).GetFileInfo(Arg.Any<string>());
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void LoadFile_OnSuccess_LoadsDataAsLogEvents()
        {
            // setup
            string logFilePath = Path.ChangeExtension(Path.Combine(TestFolder, Path.GetRandomFileName()), ".log");
            string logFileText = TestAssets.ReadResource(TestAssets.LogFile);
            File.WriteAllText(logFilePath, logFileText);

            // execute
            IFileLoadService fileLoadService = CreateService();
            LogFileLoadResult loadResult = fileLoadService.LoadFile(logFilePath);

            // assert
            Assert.That(loadResult.Success, Is.True);
            Assert.That(loadResult.LogEvents!.Count(), Is.EqualTo(300));

            LogEvent le = loadResult.LogEvents!.First();
            Assert.That(le.ServerIpAddress, Is.EqualTo("172.168.1.112"));

        }

        [Test]
        public void LoadFile_OnW3CEnumerableFailure_Success()
        {
            // setup
            string logFilePath = Path.ChangeExtension(Path.Combine(TestFolder, Path.GetRandomFileName()), ".log");
            string logFileText = TestAssets.ReadResource(TestAssets.InvalidLogFile);
            File.WriteAllText(logFilePath, logFileText);

            // execute
            IFileLoadService fileLoadService = CreateService();
            LogFileLoadResult loadResult = fileLoadService.LoadFile(logFilePath);

            // assert
            Assert.That(loadResult.Success, Is.True);
            Assert.That(loadResult.LogEvents!.Count(), Is.EqualTo(0));
        }

        private IFileLoadService CreateService(IDirectoryWrapper? dirWrapper = null, IFileWrapper? fileWrapper = null)
        {
            ILogger<FileLoadService> logger = new SubstituteBuilder<ILogger<FileLoadService>>().Build();
            dirWrapper ??= new SubstituteBuilder<IDirectoryWrapper>().Build();
            fileWrapper ??= new SubstituteBuilder<IFileWrapper>().Build();
            return new FileLoadService(logger, dirWrapper, fileWrapper);
        }


    }
}
