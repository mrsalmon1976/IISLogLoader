using IISLogLoader.Common.IO;
using IISLogLoader.Test.Common;
using NSubstitute.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLogLoader.Common.Tests.IO
{
    [TestFixture]
    public class DirectoryUtilityTests
    {
        private static string TestFolder = Path.Combine(AppContext.BaseDirectory, "DirectoryUtilityTests");

        [SetUp]
        public void SetUp() 
        {
            Directory.CreateDirectory(TestFolder);
        }

        [TearDown] 
        public void TearDown() 
        {
            Directory.Delete(TestFolder, true);
        }

        [Test]
        public void EnumerateFiles_Wildcards_ReturnsAllFiles()
        {
            // setup 
            int fileCount = RandomData.Number.Next(10, 30);
            CreateFiles(fileCount);

            // execute
            IDirectoryWrapper dirWrapper = new DirectoryWrapper();
            var files = dirWrapper.EnumerateFiles(TestFolder, "*.*").ToList();

            // assert
            Assert.That(files.Count, Is.EqualTo(fileCount));

        }

        [Test]
        public void EnumerateFilesAsync_WithSearchPattern_ReturnsSpecificFiles()
        {
            // setup 
            int randomFileCount = RandomData.Number.Next(5, 10);
            CreateFiles(randomFileCount);
            int txtFileCount = RandomData.Number.Next(11, 20);
            CreateFiles(txtFileCount, ".txt");

            // execute
            IDirectoryWrapper dirWrapper = new DirectoryWrapper();
            var files = dirWrapper.EnumerateFiles(TestFolder, "*.txt").ToList();

            // assert
            Assert.That(files.Count, Is.EqualTo(txtFileCount));
        }

        [Test]
        public void EnsureExists_FolderExists_ExecutesWithoutError()
        {
            // setup 
            string folder = RandomData.String.Letters(5, 10);
            string path = Path.Combine(TestFolder, folder);
            Directory.CreateDirectory(path);

            // execute
            IDirectoryWrapper dirWrapper = new DirectoryWrapper();
            dirWrapper.EnsureExists(path);

            // assert
            Assert.That(Directory.Exists(path));
        }

        [Test]
        public void EnsureExists_FolderNotExists_CreatesFolder()
        {
            // setup 
            string folder = RandomData.String.Letters(5, 10);
            string path = Path.Combine(TestFolder, folder);

            // execute
            IDirectoryWrapper dirWrapper = new DirectoryWrapper();
            dirWrapper.EnsureExists(path);

            // assert
            Assert.That(Directory.Exists(path));

        }

        private void CreateFiles(int fileCount, string? extension = null) 
        {
            for (int i = 0; i < fileCount; i++)
            {
                string fileName = Path.GetRandomFileName();
                string filePath = Path.Combine(TestFolder, fileName);
                if (extension != null)
                {
                    filePath = Path.ChangeExtension(filePath, extension);
                }

                string contents = RandomData.String.Paragraph();
                File.WriteAllText(filePath, contents);
            }
        }

        private static async Task<List<T>> AsyncEnumerableToList<T>(IAsyncEnumerable<T> asyncEnumerable)
        {
            if (null == asyncEnumerable)
            {
                throw new ArgumentNullException(nameof(asyncEnumerable));
            }

            var list = new List<T>();
            await foreach (var t in asyncEnumerable)
            {
                list.Add(t);
            }

            return list;
        }



    }
}
