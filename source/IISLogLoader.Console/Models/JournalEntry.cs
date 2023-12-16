using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Console.Models
{
    public class JournalEntry
    {
        public JournalEntry() 
        {
            this.FilePath = String.Empty;
        }

        public string FilePath { get; set; }

        public DateTime LastModified { get; set; }

        public long Length { get; set; }
    }
}
