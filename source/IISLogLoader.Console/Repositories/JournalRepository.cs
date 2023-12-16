using IISLogLoader.Common.IO;
using IISLogLoader.Config;
using IISLogLoader.Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Repositories
{
    public interface IJournalRepository
    {
        Journal Load(string journalName);

        void Save(Journal journal);
    }

    public class JournalRepository : IJournalRepository
    {
        private readonly string _journalFolder;
        private readonly IFileWrapper _fileWrapper;
        private readonly IDirectoryWrapper _directoryUtility;

        public const string DataFolder = "Data";

        public static JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        public JournalRepository(string appBaseFolder, IFileWrapper fileWrapper, IDirectoryWrapper directoryUtility)
        {
            _journalFolder = Path.Combine(appBaseFolder, DataFolder);
            _fileWrapper = fileWrapper;
            _directoryUtility = directoryUtility;
        }

        private string GetJournalFilePath(string journalName)
        {
            return Path.Combine(_journalFolder, $"journal-{journalName.ToLower()}.json");

        }

        public Journal Load(string journalName)
        {
            _directoryUtility.EnsureExists(_journalFolder);
            string journalFilePath = GetJournalFilePath(journalName);
            Journal journal = new Journal();
            journal.Name = journalName;
            if (_fileWrapper.Exists(journalFilePath))
            {
                string json = _fileWrapper.ReadAllText(journalFilePath);
                journal = JsonSerializer.Deserialize<Journal>(json)!;
            }
            return journal;
        }

        public void Save(Journal journal)
        {
            string journalFilePath = GetJournalFilePath(journal.Name);
            string json = JsonSerializer.Serialize(journal, DefaultJsonSerializerOptions);
            _directoryUtility.EnsureExists(_journalFolder);
            _fileWrapper.WriteAllText(journalFilePath, json);
        }
    }
}
