using IISLogLoader.Common.Data;
using IISLogLoader.Common.Models;
using IISLogLoader.Test.Common;
using IISLogLoader.Test.Common.TestDatabases.SqlServer;
using NUnit.Framework;

namespace IISLogLoader.Common.Tests.Data
{
    [TestFixture]
    [Category("RequiresDb")]
    public class SqlServerLogDbContextTests
    {
        [SetUp] 
        public void SetUp() 
        {
            // keeps the number of test files to a minimum - can't be run on TearDown as files are still locked
            SqlServerTestDbUtility dbUtility = new SqlServerTestDbUtility();
            dbUtility.DeleteTestFiles();
        }

        [Test]
        public void DeleteLogData_Integration_RemovesAllFileData()
        {
            // setup
            string tableName = RandomData.String.Letters(10, 20);
            string filePathToDelete = Path.GetRandomFileName();
            string filePathToKeep = Path.GetRandomFileName();
            int recordsToDelete = new Random().Next(10, 100);
            int recordsToKeep = new Random().Next(10, 100);

            SqlServerTestDbUtility dbUtility = new SqlServerTestDbUtility();
            string connString = dbUtility.CreateTestDatabase();

            using SqlServerLogDbContext logDbContext = new SqlServerLogDbContext(connString, tableName);
            logDbContext.RunMigrations(tableName).GetAwaiter().GetResult();
            dbUtility.SaveLogEvents(tableName, CreateLogEvents(filePathToDelete, recordsToDelete));
            dbUtility.SaveLogEvents(tableName, CreateLogEvents(filePathToKeep, recordsToKeep));

            // execute
            logDbContext.DeleteLogData(filePathToDelete).GetAwaiter().GetResult();

            // assert
            int remainingCount = dbUtility.GetRecordCount(tableName);
            Assert.That(remainingCount, Is.EqualTo(recordsToKeep));
        }

        [Test]
        public void RunMigrations_Integration_CreatesTable()
        {
            // setup
            string tableName = RandomData.String.Letters(10, 20);

            SqlServerTestDbUtility dbUtility = new SqlServerTestDbUtility();
            string connString = dbUtility.CreateTestDatabase();

            using SqlServerLogDbContext logDbContext = new SqlServerLogDbContext(connString, tableName);

            // execute
            logDbContext.RunMigrations(tableName).GetAwaiter().GetResult();

            // assert
            int remainingCount = dbUtility.GetRecordCount(tableName);
            Assert.That(remainingCount, Is.EqualTo(0));
        }

        [TestCase("IX_{{TableName}}_FilePath")]
        [TestCase("IX_{{TableName}}_DateTime")]
        public void RunMigrations_Integration_CreatesIndexes(string index)
        {
            // setup
            string tableName = RandomData.String.Letters(10, 20);
            string indexName = index.Replace("{{TableName}}", tableName);

            SqlServerTestDbUtility dbUtility = new SqlServerTestDbUtility();
            string connString = dbUtility.CreateTestDatabase();

            using SqlServerLogDbContext logDbContext = new SqlServerLogDbContext(connString, tableName);

            // execute
            logDbContext.RunMigrations(tableName).GetAwaiter().GetResult();

            // assert
            Assert.That(dbUtility.IndexExists(tableName, indexName));
        }

        [Test]
        public void SaveLogData_Integration_WritesAllRecords()
        {
            // setup
            string tableName = RandomData.String.Letters(10, 20);
            string filePath = Path.GetRandomFileName();
            int recordsToCreate = new Random().Next(10, 100);

            SqlServerTestDbUtility dbUtility = new SqlServerTestDbUtility();
            string connString = dbUtility.CreateTestDatabase();

            using SqlServerLogDbContext logDbContext = new SqlServerLogDbContext(connString, tableName);
            logDbContext.RunMigrations(tableName).GetAwaiter().GetResult();
            var logEvents = CreateLogEvents(filePath, recordsToCreate);

            // execute
            logDbContext.SaveLogData(logEvents).GetAwaiter().GetResult();

            // assert
            int recordCount = dbUtility.GetRecordCount(tableName);
            Assert.That(recordCount, Is.EqualTo(recordsToCreate));
        }

        private IEnumerable<LogEvent> CreateLogEvents(string filePath, int eventCount)
        {
            List<LogEvent> logEvents = new List<LogEvent>();
            for (int i = 0; i < eventCount; i++)
            {
                LogEvent logEvent = new SubstituteBuilder<LogEvent>()
                    .WithRandomProperties()
                    .WithProperty<string?>(x => x.FilePath, filePath)
                    .WithProperty<string?>(x => x.ClientIpAddress, RandomData.Internet.IPAddress().ToString())
                    .WithProperty<string?>(x => x.Method, RandomData.Internet.HttpMethod().ToString())
                    .WithProperty<string?>(x => x.ProtocolVersion, RandomData.String.Letters(50))
                    .WithProperty<string?>(x => x.ServerIpAddress, RandomData.Internet.IPAddress().ToString())
                    .WithProperty<int?>(x => x.StatusCode, (int)RandomData.Internet.HttpStatusCode())
                    .WithProperty<string?>(x => x.ProtocolSubstatus, RandomData.String.Letters(100))
                    .WithProperty<string?>(x => x.WindowsStatusCode, RandomData.String.Letters(100))
                    .Build();

                logEvents.Add(logEvent);
            }
            return logEvents;
        }

    }
}
