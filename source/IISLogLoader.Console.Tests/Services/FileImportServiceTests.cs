using IISLogLoader.Common.Data;
using IISLogLoader.Common.Models;
using IISLogLoader.Console.Services;
using IISLogLoader.Test.Common;
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
    public class FileImportServiceTests
    {
        [Test]
        public void ImportLogFile_OnSuccess_SavesLogEventsFromFile()
        {
            // setup
            ILogDbContext logDbContext = new SubstituteBuilder<ILogDbContext>().Build();
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());

            IEnumerable<LogEvent> logEvents = CreateLogEvents(RandomData.Number.Next(3, 10));
            LogFileLoadResult loadResult = new LogFileLoadResult();
            loadResult.Success = true;
            loadResult.LogEvents = logEvents;

            IFileLoadService fileLoadService = new SubstituteBuilder<IFileLoadService>().Build();
            fileLoadService.LoadFile(filePath).Returns(loadResult);

            // execute
            IFileImportService fileImportService = CreateService(fileLoadService);
            LogFileLoadResult result = fileImportService.ImportLogFile(logDbContext, filePath).GetAwaiter().GetResult();

            // assert
            fileLoadService.Received(1).LoadFile(filePath);
            logDbContext.Received(1).DeleteLogData(filePath);
            logDbContext.Received(1).SaveLogData(logEvents);
            Assert.That(result, Is.EqualTo(loadResult));
        }

        [Test]
        public void ImportLogFile_OnFailure_DoesNotUpdateDatabase()
        {
            // setup
            ILogDbContext logDbContext = new SubstituteBuilder<ILogDbContext>().Build();
            string filePath = Path.Combine(AppContext.BaseDirectory, Path.GetRandomFileName());

            LogFileLoadResult loadResult = new LogFileLoadResult();
            loadResult.Success = false;

            IFileLoadService fileLoadService = new SubstituteBuilder<IFileLoadService>().Build();
            fileLoadService.LoadFile(filePath).Returns(loadResult);

            // execute
            IFileImportService fileImportService = CreateService(fileLoadService);
            LogFileLoadResult result = fileImportService.ImportLogFile(logDbContext, filePath).GetAwaiter().GetResult();

            // assert
            fileLoadService.Received(1).LoadFile(filePath);
            logDbContext.DidNotReceive().DeleteLogData(Arg.Any<string>());
            logDbContext.DidNotReceive().SaveLogData(Arg.Any<IEnumerable<LogEvent>>());
            Assert.That(result, Is.EqualTo(loadResult));
        }

        private IEnumerable<LogEvent> CreateLogEvents(int count)
        {
            List<LogEvent> logEvents = new List<LogEvent>();
            for (int i=0; i<count; i++)
            {
                logEvents.Add(new SubstituteBuilder<LogEvent>().Build());

            }
            return logEvents;
        }

        private IFileImportService CreateService(IFileLoadService? fileLoadService = null)
        {
            fileLoadService ??= new SubstituteBuilder<IFileLoadService>().Build();
            return new FileImportService(fileLoadService);
        }
    }
}
