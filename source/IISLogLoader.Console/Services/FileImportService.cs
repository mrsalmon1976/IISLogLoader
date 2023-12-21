using IISLogLoader.Common.Data;
using IISLogLoader.Common.Models;
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
        Task<LogFileLoadResult> ImportLogFile(ILogDbContext dbContext, string filePath);
    }

    public class FileImportService : IFileImportService
    {
        private readonly IFileLoadService _fileLoadService;

        public FileImportService(IFileLoadService fileLoadService)
        {
            _fileLoadService = fileLoadService;
        }

        public async Task<LogFileLoadResult> ImportLogFile(ILogDbContext dbContext, string filePath)
        {
            var loadResult = _fileLoadService.LoadFile(filePath);
            if (loadResult.Success)
            {
                await dbContext.DeleteLogData(filePath);
                await dbContext.SaveLogData(loadResult.LogEvents!);
            }
            return loadResult;
        }
    }
}
