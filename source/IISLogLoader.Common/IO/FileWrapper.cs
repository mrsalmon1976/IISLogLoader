using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common.IO
{

    public interface IFileWrapper
    {
        bool Exists(string path);

        FileInfoWrapper GetFileInfo(string path);

        string ReadAllText(string path);

        void WriteAllText(string path, string? contents);

    }

    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public FileInfoWrapper GetFileInfo(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return new FileInfoWrapper(fileInfo);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);  
        }

        public void WriteAllText(string path, string? contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}
