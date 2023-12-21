using IISLogLoader.Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Models
{
    public class Journal
    {
        private List<JournalEntry> _journalEntries = new List<JournalEntry>();

        public Journal() 
        {
            this.Name = string.Empty;
        }

        public virtual IEnumerable<JournalEntry> Files
        {
            get
            {
                return _journalEntries;
            }
            set
            {
                _journalEntries = new List<JournalEntry>(value);
            }
        }

        public virtual string Name { get; set; }

        public virtual JournalEntry? GetEntry(string filePath)
        {
            return this.Files.SingleOrDefault(x => x.FilePath == filePath);
        }

        public virtual void Update(FileInfoWrapper fileInfo, bool success, string? errorMessage)
        {
            JournalEntry? journalEntry = GetEntry(fileInfo.FullName);
            if (journalEntry == null)
            {
                journalEntry = new JournalEntry();
                _journalEntries.Add(journalEntry);
            }
            journalEntry.Length = fileInfo.Length;
            journalEntry.LastModified = fileInfo.LastWriteTime;
            journalEntry.FilePath = fileInfo.FullName;
            journalEntry.Success = success;
            journalEntry.ErrorMessage = errorMessage;
        }

    }
}
