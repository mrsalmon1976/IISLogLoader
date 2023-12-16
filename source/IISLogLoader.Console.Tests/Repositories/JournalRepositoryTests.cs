using IISLogLoader.Common.IO;
using IISLogLoader.Console.Models;
using IISLogLoader.Console.Repositories;
using IISLogLoader.Test.Common;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Tests.Repositories
{
    [TestFixture]
    public class JournalRepositoryTests
    {
        private static string DefaultBaseFolder = AppContext.BaseDirectory;

        [Test]
        public void Load_EnsuresDirectoryExists()
        {
            // setup
            IDirectoryWrapper dirWrapper = new SubstituteBuilder<IDirectoryWrapper>().Build();

            // execute
            IJournalRepository journalRepo = CreateJournalRepository(dirWrapper: dirWrapper);
            journalRepo.Load(RandomData.String.Letters(2, 10));

            //assert
            string expectedJournalFolder = Path.Combine(DefaultBaseFolder, JournalRepository.DataFolder);
            dirWrapper.Received(1).EnsureExists(expectedJournalFolder);
        }


        [Test]
        public void Load_FileExists_LoadsFromFile()
        {
            // setup
            string journalName = RandomData.String.Letters(2, 10);
            string journalFilePath = GetExpectedJournalFolder(DefaultBaseFolder, journalName);
            Journal journal = new SubstituteBuilder<Journal>().WithRandomProperties().Build();
            string journalJson = JsonSerializer.Serialize(journal);

            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();
            fileWrapper.Exists(journalFilePath).Returns(true);
            fileWrapper.ReadAllText(journalFilePath).Returns(journalJson);

            // execute
            IJournalRepository journalRepo = CreateJournalRepository(fileWrapper: fileWrapper);
            Journal result = journalRepo.Load(journalName);

            //assert
            fileWrapper.Received(1).Exists(journalFilePath);
            fileWrapper.Received(1).ReadAllText(journalFilePath);
            Assert.That(result.Name, Is.EqualTo(journal.Name));
        }

        [Test]
        public void Load_FileNotExists_LoadsDefaultJournal()
        {
            // setup
            string journalName = RandomData.String.Letters(2, 10);

            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();
            fileWrapper.Exists(Arg.Any<string>()).Returns(false);

            // execute
            IJournalRepository journalRepo = CreateJournalRepository(fileWrapper: fileWrapper);
            Journal result = journalRepo.Load(journalName);

            //assert
            fileWrapper.Received(1).Exists(Arg.Any<String>());
            fileWrapper.DidNotReceive().ReadAllText(Arg.Any<String>());
            Assert.That(result.Name, Is.EqualTo(journalName));
        }

        [Test]
        public void Save_OnExecute_WritesToDisk()
        {
            // setup
            Journal journal = new SubstituteBuilder<Journal>().WithRandomProperties().Build();
            string journalFolder = Path.Combine(DefaultBaseFolder, JournalRepository.DataFolder);
            string journalFilePath = GetExpectedJournalFolder(DefaultBaseFolder, journal.Name);
            string journalJson = JsonSerializer.Serialize(journal, JournalRepository.DefaultJsonSerializerOptions);

            IDirectoryWrapper dirWrapper = new SubstituteBuilder<IDirectoryWrapper>().Build();
            IFileWrapper fileWrapper = new SubstituteBuilder<IFileWrapper>().Build();

            // execute
            IJournalRepository journalRepo = CreateJournalRepository(fileWrapper: fileWrapper, dirWrapper: dirWrapper);
            journalRepo.Save(journal);

            // assert
            dirWrapper.Received(1).EnsureExists(journalFolder);
            fileWrapper.Received(1).WriteAllText(journalFilePath, journalJson);
        }


        private JournalRepository CreateJournalRepository(string? appBaseFolder = null, IFileWrapper? fileWrapper = null, IDirectoryWrapper? dirWrapper = null)
        {
            appBaseFolder ??= DefaultBaseFolder;
            fileWrapper ??= new SubstituteBuilder<IFileWrapper>().Build();
            dirWrapper ??= new SubstituteBuilder<IDirectoryWrapper>().Build();
            return new JournalRepository(appBaseFolder, fileWrapper, dirWrapper);
        }

        private static string GetExpectedJournalFolder(string baseFolder, string journalName)
        {
            return Path.Combine(baseFolder, JournalRepository.DataFolder, $"journal-{journalName.ToLower()}.json");

        }

    }
}
