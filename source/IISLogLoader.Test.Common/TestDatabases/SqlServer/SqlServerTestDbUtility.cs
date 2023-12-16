using Dapper;
using IISLogLoader.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Test.Common.TestDatabases.SqlServer
{
    public class SqlServerTestDbUtility
    {
        private readonly string _testDataFolder;
        private readonly string _testDbPath;
        private readonly string _testLogPath;
        private string? _connectionString;

        public SqlServerTestDbUtility() 
        {
            _testDataFolder = Path.Combine(AppContext.BaseDirectory, "TestDatabases", "SqlServer");
            _testDbPath = Path.Combine(_testDataFolder, "SqlServerTestDb.mdf");
            _testLogPath = Path.Combine(_testDataFolder, "SqlServerTestDb_log.ldf");

            if (!File.Exists(_testDbPath))
            {
                throw new FileNotFoundException("Database file not found", _testDbPath);
            }
            if (!File.Exists(_testLogPath))
            {
                throw new FileNotFoundException("Database log file not found", _testLogPath);
            }

        }

        private SqlConnection CreateConnection()
        {
            if (_connectionString == null)
            {
                throw new InvalidOperationException("CreateTestDatabase has not been called to initialise a new test database.");
            }

            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;

        }

        public string CreateTestDatabase()
        { 

            string filePrefix = Path.GetRandomFileName();
            DbDataPath = _testDbPath.Replace("SqlServerTestDb", $"{filePrefix}_SqlServerTestDb");
            DbLogPath = _testLogPath.Replace("SqlServerTestDb", $"{filePrefix}_SqlServerTestDb");
            _connectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFileName={DbDataPath};Integrated Security=true;";

            // create the file copies
            File.Copy(_testDbPath, DbDataPath, true);
            File.Copy(_testLogPath, DbLogPath, true);
            TestDatabaseConnection();
            return _connectionString;

        }

        public string? DbDataPath { get; private set; }

        public string? DbLogPath { get; private set; }

        public void SaveLogEvents(string tableName, IEnumerable<LogEvent> logEvents)
        {
            TestDatabaseConnection();
            string sql = $@"INSERT INTO {tableName}
                           ([FilePath]
                           ,[DateTime]
                           ,[ClientIpAddress]
                           ,[BytesReceived]
                           ,[Cookie]
                           ,[Host]
                           ,[Method]
                           ,[Referrer]
                           ,[UriQuery]
                           ,[UriStem]
                           ,[UserAgent]
                           ,[UserName]
                           ,[ProtocolVersion]
                           ,[ServerName]
                           ,[ServerIpAddress]
                           ,[ServerPort]
                           ,[SiteName]
                           ,[BytesSent]
                           ,[StatusCode]
                           ,[ProtocolSubstatus]
                           ,[WindowsStatusCode]
                           ,[TimeTaken])
                     VALUES
                           (@FilePath
                           ,@DateTime
                           ,@ClientIpAddress
                           ,@BytesReceived
                           ,@Cookie
                           ,@Host
                           ,@Method
                           ,@Referrer
                           ,@UriQuery
                           ,@UriStem
                           ,@UserAgent
                           ,@UserName
                           ,@ProtocolVersion
                           ,@ServerName
                           ,@ServerIpAddress
                           ,@ServerPort
                           ,@SiteName
                           ,@BytesSent
                           ,@StatusCode
                           ,@ProtocolSubstatus
                           ,@WindowsStatusCode
                           ,@TimeTaken)";

            using (IDbConnection conn = CreateConnection()) 
            {
                foreach (LogEvent logEvent in logEvents)
                {
                    conn.Execute(sql, logEvent);
                }
                conn.Close();
            }
        }


        public void DeleteTestFiles()
        {
            var files = Directory.EnumerateFiles(_testDataFolder);
            foreach (string file in files)
            {
                if (file.Equals(_testDbPath) || file.Equals(_testLogPath)) 
                { 
                    continue; 
                }
                try 
                {
                    File.Delete(file);
                }
                catch (IOException ex)
                {
                    // we just swallow this - the DB file remains locked until the engine releases, so we'll get it eventually
                }
            }
        }

        public int GetRecordCount(string tableName)
        {
            int result = -1;
            using (IDbConnection conn = CreateConnection())
            {
                string sql = $"SELECT COUNT(*) FROM [{tableName}]";
                result = conn.ExecuteScalar<int>(sql);
                conn.Close();
            }
            return result;
        }

        public int GetRecordCount(string tableName, string filePath)
        {
            int result = -1;
            using (IDbConnection conn = CreateConnection())
            {
                string sql = $"SELECT COUNT(*) FROM {tableName} WHERE FilePath = @FilePath";
                result = conn.ExecuteScalar<int>(sql, new { FilePath = filePath });
                conn.Close();
            }
            return result;
        }

        public bool IndexExists(string tableName, string indexName)
        {
            int result = -1;
            string sql = $"SELECT COUNT(*) FROM sys.indexes WHERE name='{indexName}' AND object_id = OBJECT_ID('{tableName}')";
            using (IDbConnection conn = CreateConnection())
            {
                result = conn.ExecuteScalar<int>(sql);
                conn.Close();
            }
            return (result == 1);
        }

        public void TestDatabaseConnection()
        {
            using (SqlConnection conn = CreateConnection())
            {
                conn.Close();
            }

        }
    }
}
