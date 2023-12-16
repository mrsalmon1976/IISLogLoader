using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common
{
    public class LogLoaderDbType
    {
        public const string SqlServer = "sqlserver";

        public static string[] AllTypes = { SqlServer };
    }
}
