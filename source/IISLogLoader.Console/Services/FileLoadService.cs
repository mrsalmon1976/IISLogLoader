using IISLogLoader.Common.IO;
using IISLogLoader.Common.Models;
using IISLogLoader.Common.Models.Mappers;
using IISLogLoader.Console.Models;
using Tx.Windows;

namespace IISLogLoader.Console.Services
{
    public interface IFileLoadService
    {
        Task<IEnumerable<FileInfoWrapper>> FindModifiedFiles(string folder, string fileSearchPattern, Journal journal);

        Task<IEnumerable<LogEvent>> LoadFile(string filePath);
    }

    public class FileLoadService : IFileLoadService
    {
        private readonly IDirectoryWrapper _directoryUtility;
        private readonly IFileWrapper _fileWrapper;

        public FileLoadService(IDirectoryWrapper directoryUtility, IFileWrapper fileWrapper)
        {
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

        public async Task<IEnumerable<LogEvent>> LoadFile(string filePath)
        {
            return await Task.Run(() => LogEventMapper.MapFromW3CEvents(filePath, W3CEnumerable.FromFile(filePath)));
        }


    }
}
