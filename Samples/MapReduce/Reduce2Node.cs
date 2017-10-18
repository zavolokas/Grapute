using System;
using System.IO;
using Grapute;

namespace MapReduce
{
    class Reduce2Node: Node<FileInfo, string>
    {
        protected override string[] Process(FileInfo fileInfo)
        {
            var dirName = new DirectoryInfo(fileInfo.DirectoryName).Parent.FullName;
            var fileName = Path.Combine(dirName, $"RESULT.txt");
            Token currentToken = null;

            using (var streamReader = fileInfo.OpenText())
            using (var streamWriter = new StreamWriter(fileName))
            {
                while (!streamReader.EndOfStream)
                {
                    var token = Token.ParseToken(streamReader.ReadLine());
                    if (currentToken == null
                        || !string.Equals(currentToken.Term, token.Term, StringComparison.InvariantCultureIgnoreCase)
                        || !string.Equals(currentToken.Doc, token.Doc, StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentToken?.Write(streamWriter);
                        currentToken = token;
                    }
                    else if (string.Equals(currentToken.Term, token.Term, StringComparison.InvariantCultureIgnoreCase)
                             && string.Equals(currentToken.Doc, token.Doc, StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentToken.Count += token.Count;
                    }
                }
                streamWriter.Close();
            }
            return new[] { fileName };
        }
    }
}