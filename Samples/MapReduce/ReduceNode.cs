using System;
using System.IO;
using Grapute;

namespace MapReduce
{
    internal class ReduceNode : Node<FileInfo, FileInfo>
    {
        protected override FileInfo[] Process(FileInfo fileInfo)
        {
            var dirName = fileInfo.DirectoryName;
            var fileName = Path.Combine(dirName,
                Path.GetFileNameWithoutExtension(fileInfo.Name) + $"_REDUCE.txt");
            Token currentToken = null;

            using (var streamReader = fileInfo.OpenText())
            using (var streamWriter = new StreamWriter(fileName))
            {
                while (!streamReader.EndOfStream)
                {
                    var token = Token.ParseToken(streamReader.ReadLine());
                    if (currentToken == null || currentToken.Term != token.Term)
                    {
                        currentToken?.Write(streamWriter);
                        currentToken = token;
                    }
                    else if (string.Equals(currentToken.Term, token.Term,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentToken.Count += token.Count;
                    }
                }
                streamWriter.Close();
            }
            return new[] { new FileInfo(fileName) };
        }
    }
}