using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Models
{
    public class LogStorageInfo
    {
        public const string DefaultFileSearchPattern = "*.log";

        public LogStorageInfo()
        {
            this.ConnectionString = String.Empty;
            this.FileSearchPattern = DefaultFileSearchPattern;
            this.DbType = String.Empty;
            this.Name = String.Empty;
            this.WatchFolders = new string[] { };
            this.TableName = String.Empty;
        }

        public string DbType { get; set; }

        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public string FileSearchPattern { get; set; }

        public string TableName { get; set; }

        public string[] WatchFolders {  get; set; }   
    }
}
