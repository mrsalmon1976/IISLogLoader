using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common.Models
{
    public class LogFileLoadResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public IEnumerable<LogEvent>? LogEvents { get; set; }
    }
}
