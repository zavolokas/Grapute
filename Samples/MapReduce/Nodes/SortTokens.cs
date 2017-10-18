using System.Collections.Generic;
using System.IO;
using Grapute;

namespace MapReduce
{
    internal class SortTokens : Node<FileInfo, FileInfo>
    {
        protected override FileInfo[] Process(FileInfo fileInfo)
        {
            // SORT. sort tokens
            var dirName = fileInfo.DirectoryName;
            var fileName = Path.Combine(dirName, Path.GetFileNameWithoutExtension(fileInfo.Name) + $"_SORT.txt");
            var tokens = new List<Token>();

            using (var streamReader = fileInfo.OpenText())
            {
                while (!streamReader.EndOfStream)
                {
                    var token = Token.ParseToken(streamReader.ReadLine());
                    tokens.Add(token);
                }
            }

            tokens.Sort(new TokenComparer());

            using (var streamWriter = new StreamWriter(fileName))
            {
                for (var i = 0; i < tokens.Count; i++)
                {
                    tokens[i].Write(streamWriter);
                }

                streamWriter.Close();
            }

            return new[] { new FileInfo(fileName) };
        }
    }
}