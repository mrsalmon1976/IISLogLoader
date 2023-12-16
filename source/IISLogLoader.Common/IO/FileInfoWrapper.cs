using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common.IO
{
    public class FileInfoWrapper
    {
        public FileInfoWrapper() 
        {
            this.FullName = String.Empty;
            this.Name = String.Empty;
        }

        public FileInfoWrapper(FileInfo fileInfo) 
        {
            this.FullName = fileInfo.FullName;
            this.LastWriteTime = fileInfo.LastWriteTime;
            this.Length = fileInfo.Length;
            this.Name = fileInfo.Name;
        }

        public string FullName { get; set; }

        public virtual DateTime LastWriteTime { get; set; }

        public virtual long Length { get; set; }

        public virtual string Name { get; set; }

    }
}
