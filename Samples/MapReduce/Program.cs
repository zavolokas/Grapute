using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grapute;

namespace MapReduce
{
    class Program
    {
        static void Main(string[] args)
        {
            // Node1 
            // input to path to directory with documents
            // output - array of paths to text files
            var startNode = new FuncNode<string, FileInfo>(x =>
            {
                var dir = new DirectoryInfo(x);

                var files = dir.EnumerateFiles("*.txt").ToArray();

                if (files.Length > 0)
                {
                    var fileInfo = files[0];
                    string dirName = Path.Combine(fileInfo.DirectoryName, "mr");
                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);
                }

                return files;
            });

            startNode.SetInput("..\\..\\docs");

            startNode
                .CollectAllOutputsToOneArray()
                .ForArray(fileInfos =>
                {
                    // split file into many
                    var resultFileNames = new List<FileInfo>();
                    int partitionMaxSize = 100_000;
                    char[] buffer = new char[1];

                    for (int i = 0; i < fileInfos.Length; i++)
                    {
                        var fileInfo = fileInfos[i];
                        int currentFileSize = 0;
                        int fileIndex = 0;

                        string dirName = Path.Combine(fileInfo.DirectoryName, "mr");
                        string fileName = GenerateFileName(dirName, fileInfo, fileIndex);

                        var streamWriter = new StreamWriter(fileName);
                        resultFileNames.Add(new FileInfo(fileName));

                        bool isLastPunctuation = false;

                        using (var streamReader = fileInfo.OpenText())
                        {
                            while (!streamReader.EndOfStream)
                            {
                                streamReader.Read(buffer, 0, 1);
                                char currentChar = buffer[0];
                                bool isPunctuation = Char.IsPunctuation(currentChar) || Char.IsWhiteSpace(currentChar);
                                if (Char.IsLetter(currentChar) || (isPunctuation && !isLastPunctuation))
                                {
                                    currentFileSize++;
                                    isLastPunctuation = isPunctuation;

                                    if (isPunctuation)
                                        currentChar = (char)13;
                                    else if (!Char.IsLower(currentChar))
                                        currentChar = Char.ToLower(currentChar);

                                    streamWriter.Write(new[] { currentChar }, 0, 1);

                                    if (currentFileSize > partitionMaxSize && isPunctuation)
                                    {
                                        fileIndex++;
                                        fileName = GenerateFileName(dirName, fileInfo, fileIndex);
                                        streamWriter.Close();
                                        streamWriter.Dispose();
                                        streamWriter = new StreamWriter(fileName);
                                        resultFileNames.Add(new FileInfo(fileName));
                                        currentFileSize = 0;
                                    }
                                }
                            }
                            streamWriter.Close();
                            streamWriter.Dispose();
                        }
                    }

                    return resultFileNames.ToArray();
                })
                .ForEachOutput(fileInfo =>
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
                            var token = new Token { Term = streamReader.ReadLine(), Count = 1, Doc = fileName};
                            token.Write(streamWriter);
                        }
                        streamWriter.Close();
                        streamWriter.Dispose();
                    }
                    return new[] { new FileInfo(fullFileName) };
                })
                .ForEachOutput(fileInfo =>
                {
                    // SORT. Read tokens and its counts
                    var dirName = fileInfo.DirectoryName;
                    var fileName = Path.Combine(dirName,
                        Path.GetFileNameWithoutExtension(fileInfo.Name) + $"_SORT.txt");
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
                        for (int i = 0; i < tokens.Count; i++)
                        {
                            tokens[i].Write(streamWriter);
                        }

                        streamWriter.Close();
                    }

                    return new[] { new FileInfo(fileName) };
                })
                .ForEachOutput(fileInfo =>
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
                    return new[] { new FileInfo(fileName)};
                })
                .CollectAllOutputsToOneArray()
                .ForArray(fileInfos =>
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
                })
                .ForEachOutput(fileInfo =>
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
                })
                .ForEachOutput(f =>
                {
                    Console.WriteLine(f);
                    return new int[] { };
                })
                .Process();
        }

        private static string GenerateFileName(string dirName, FileInfo f, int fileIndex)
        {
            return Path.Combine(dirName, Path.GetFileNameWithoutExtension(f.Name) + $"_{fileIndex}.txt");
        }
    }
}