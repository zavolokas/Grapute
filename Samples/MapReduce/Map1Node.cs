using System.IO;
using System.Linq;
using Grapute;

namespace MapReduce
{
    class Map1Node:Node<FileInfo, FileInfo>
    {
        protected override FileInfo[] Process(FileInfo fileInfo)
        {
            // MAP. Read tokens and write back in form of (token count=1)
            var dirName = fileInfo.DirectoryName;
            var originalFileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            var fileNameItems = originalFileName.Split('_').ToList();
            fileNameItems.RemoveAt(fileNameItems.Count - 1);
            var fileName = fileNameItems.ToArray().Aggregate((a, b) => $"{a}{b}");
            var fullFileName = Path.Combine(dirName, originalFileName + $"_MAP.txt");
            using (var streamWriter = new StreamWriter(fullFileName))
            using (var streamReader = fileInfo.OpenText())
            {
                while (!streamReader.EndOfStream)
                {
                    var token = new Token { Term = streamReader.ReadLine(), Count = 1, Doc = fileName };
                    token.Write(streamWriter);
                }
                streamWriter.Close();
                streamWriter.Dispose();
            }
            return new[] { new FileInfo(fullFileName) };
        }
    }
}