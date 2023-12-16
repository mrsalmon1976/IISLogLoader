using IISLogLoader.Common;
using IISLogLoader.Common.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Factories
{
    public interface ILogDbContextFactory
    {
        ILogDbContext GetDbContext(string dbType, string connectionString, string tableName);
    }

    public class LogDbContextFactory : ILogDbContextFactory
    {
        public ILogDbContext GetDbContext(string dbType, string connectionString, string tableName)
        {
            switch ((dbType ?? String.Empty).ToLower())
            {
                case LogLoaderDbType.SqlServer:
                    return new SqlServerLogDbContext(connectionString, tableName);
                default:
                    throw new NotSupportedException($"Database type '{dbType}' not supported.");
            }
        }
    }
}
