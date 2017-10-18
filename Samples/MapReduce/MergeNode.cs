using System.IO;
using Grapute;

namespace MapReduce
{
    class MergeNode:Node<FileInfo[], FileInfo>
    {
        protected override FileInfo[] Process(FileInfo[] fileInfos)
        {
            var prevResultFile = fileInfos[0].FullName;

            if (fileInfos.Length > 1)
            {
                var dirName = fileInfos[0].DirectoryName;
                var fileName = Path.Combine(dirName, "ALL.txt");

                var comp = new TokenComparer();

                for (int i = 1; i < fileInfos.Length; i++)
                {
                    using (var streamWriter = new StreamWriter(fileName))
                    using (var streamReader1 = new FileInfo(prevResultFile).OpenText())
                    using (var streamReader2 = fileInfos[i].OpenText())
                    {
                        Token token1 = null;
                        Token token2 = null;

                        while (!streamReader1.EndOfStream && !streamReader2.EndOfStream)
                        {
                            if (token1 == null && !streamReader1.EndOfStream)
                                token1 = Token.ParseToken(streamReader1.ReadLine());

                            if (token2 == null && !streamReader2.EndOfStream)
                                token2 = Token.ParseToken(streamReader2.ReadLine());

                            var result = comp.Compare(token1, token2);
                            if (result < 0)
                            {
                                token1.Write(streamWriter);
                                token1 = null;
                            }
                            else
                            {
                                token2.Write(streamWriter);
                                token2 = null;
                            }
                        }
                        streamWriter.Flush();

                        prevResultFile = fileName;
                        fileName = Path.Combine(dirName, $"ALL_{i}.txt");
                    }
                }
            }
            return new[] { new FileInfo(prevResultFile) };
        }
    }
}