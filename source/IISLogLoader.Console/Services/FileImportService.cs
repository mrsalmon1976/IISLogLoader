using IISLogLoader.Common.Data;
using IISLogLoader.Console.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tx.Windows;

namespace IISLogLoader.Console.Services
{
    public interface IFileImportService
    {
        Task ImportLogFile(ILogDbContext dbContext, string filePath);
    }

    public class FileImportService : IFileImportService
    {
        private readonly IFileLoadService _fileLoadService;

        public FileImportService(IFileLoadService fileLoadService)
        {
            _fileLoadService = fileLoadService;
        }

        public async Task ImportLogFile(ILogDbContext dbContext, string filePath)
        {
            var events = await _fileLoadService.LoadFile(filePath);
            await dbContext.DeleteLogData(filePath);
            await dbContext.SaveLogData(events);
        }
    }
}
