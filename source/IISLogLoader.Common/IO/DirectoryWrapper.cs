using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common.IO
{
    public interface IDirectoryWrapper
    {
        IEnumerable<string> EnumerateFiles(string path, string searchPattern);

        void EnsureExists(string path);
    }

    public class DirectoryWrapper : IDirectoryWrapper
    {
        public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return Directory.EnumerateFiles(path, searchPattern);
        }

        public void EnsureExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

    }
}
