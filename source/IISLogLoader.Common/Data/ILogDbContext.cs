using IISLogLoader.Common.Models;

namespace IISLogLoader.Common.Data
{
    public interface ILogDbContext : IDisposable
    {

        void BeginTransaction();

        void Commit();

        Task DeleteLogData(string filePath);

        Task SaveLogData(IEnumerable<LogEvent> logEvents);


        void Rollback();

        Task RunMigrations(string tableName);
    }

}
