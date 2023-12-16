using IISLogLoader.Common.IO;
using IISLogLoader.Common.Models;
using System.Data;
using System.Data.SqlClient;

namespace IISLogLoader.Common.Data
{
    public class SqlServerLogDbContext : ILogDbContext
    {
        private SqlConnection _sqlConnection;
        private SqlTransaction? _transaction;
        private readonly string _tableName;

        public SqlServerLogDbContext(string connectionString, string tableName)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _tableName = tableName;
        }

        public void BeginTransaction()
        {
            _transaction = _sqlConnection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public async Task DeleteLogData(string filePath)
        {
            EnsureConnectionOpen();
            string sql = $"DELETE FROM {_tableName} WHERE FilePath = @FilePath";
            using (SqlCommand cmd = new SqlCommand(sql, _sqlConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@FilePath", filePath));
                cmd.Transaction = _transaction;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public void Dispose()
        {
            if (_transaction != null) 
            { 
                _transaction.Dispose(); 
            }
            if (_sqlConnection.State == ConnectionState.Open)
            {
                _sqlConnection.Close();
            }
            _sqlConnection.Dispose();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public Task RunMigrations(string tableName)
        {
            EnsureConnectionOpen();
            string indexName = tableName.Replace(".", "_");
            string sql = ResourceUtility.LoadResource("IISLogLoader.Common.Data.Resources.SqlServer.sql");
            sql = sql.Replace("{{TableName}}", tableName).Replace("{{IndexName}}", indexName);
            using (SqlCommand cmd = _sqlConnection.CreateCommand())
            {
                cmd.CommandText = sql;
                return cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task SaveLogData(IEnumerable<LogEvent> logEvents)
        {
            EnsureConnectionOpen();
            using DataTable dt = CreateDataTableFromEvents(logEvents);
            using SqlBulkCopy sqlBulkCopy = CreateBulkCopy(dt.Columns);
            await sqlBulkCopy.WriteToServerAsync(dt);
        }


        private SqlBulkCopy CreateBulkCopy(DataColumnCollection dataColumns)
        {
            EnsureConnectionOpen();
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(_sqlConnection, SqlBulkCopyOptions.Default, _transaction);
            sqlBulkCopy.DestinationTableName = _tableName;
            for (int i = 0; i < dataColumns.Count; i++)
            {
                string columnName = dataColumns[i].ColumnName;
                sqlBulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(columnName, columnName));
            }
            return sqlBulkCopy;
        }

        private DataTable CreateDataTableFromEvents(IEnumerable<LogEvent> logEvents)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FilePath");
            dt.Columns.Add("DateTime");
            dt.Columns.Add("ClientIpAddress");
            dt.Columns.Add("BytesReceived");
		    dt.Columns.Add("Cookie");
            dt.Columns.Add("Host");
            dt.Columns.Add("Method");
            dt.Columns.Add("Referrer");
            dt.Columns.Add("UriQuery");
            dt.Columns.Add("UriStem");
            dt.Columns.Add("UserAgent");
            dt.Columns.Add("UserName");
            dt.Columns.Add("ProtocolVersion");
            dt.Columns.Add("ServerName");
            dt.Columns.Add("ServerIpAddress");
            dt.Columns.Add("ServerPort");
            dt.Columns.Add("SiteName");
            dt.Columns.Add("BytesSent");
            dt.Columns.Add("StatusCode");
            dt.Columns.Add("ProtocolSubstatus");
            dt.Columns.Add("WindowsStatusCode");
            dt.Columns.Add("TimeTaken");

            foreach (LogEvent logEvent in logEvents)
            {
                dt.Rows.Add(
                    logEvent.FilePath, 
                    logEvent.DateTime,
                    logEvent.ClientIpAddress,
                    logEvent.BytesReceived,
                    logEvent.Cookie,
                    logEvent.Host,
                    logEvent.Method,
                    logEvent.Referrer,
                    logEvent.UriQuery,
                    logEvent.UriStem,
                    logEvent.UserAgent,
                    logEvent.UserName,
                    logEvent.ProtocolVersion,
                    logEvent.ServerName,
                    logEvent.ServerIpAddress,
                    logEvent.ServerPort,
                    logEvent.SiteName,
                    logEvent.BytesSent,
                    logEvent.StatusCode,
                    logEvent.ProtocolSubstatus,
                    logEvent.WindowsStatusCode,
                    logEvent.TimeTaken
                    );
            }
            return dt;
        }

        private void EnsureConnectionOpen()
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }
        }
    }
}
