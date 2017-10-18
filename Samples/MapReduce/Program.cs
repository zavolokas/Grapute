using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
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
                return dir.EnumerateFiles("*.txt").ToArray();
            });

            startNode.SetInput("..\\..\\docs");

            startNode
                .ForEachOutput(fileInfo =>
                {
                    // split file into many
                    var resultFileNames = new List<FileInfo>();
                    int partitionMaxSize = 100_000;
                    char[] buffer = new char[1];
                    int currentFileSize = 0;
                    int fileIndex = 0;

                    string dirName = Path.Combine(fileInfo.DirectoryName, "mr");
                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);

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
                                    currentChar = (char) 13;
                                else if (!Char.IsLower(currentChar))
                                    currentChar = Char.ToLower(currentChar);

                                streamWriter.Write(new[] {currentChar}, 0, 1);

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
                            var token = new Token {Term = streamReader.ReadLine(), Count = 1, Doc = fileName};
                            token.Write(streamWriter);
                        }
                        streamWriter.Close();
                        streamWriter.Dispose();
                    }
                    return new[] {new FileInfo(fullFileName)};
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

                    return new[] {new FileInfo(fileName)};
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
                    return new[] {new FileInfo(fileName)};
                })
                //.ForEachOutput(f =>
                //{
                //    //term = key (docCount(that contain specific token), docId, countInDoc) TOTAL_TOKEN_COUNT TOTAL_DOC_COUNT
                //    //tf = countInDoc(specific token count in document) / TOTAL_TOKEN_COUNT(in document)
                //    //idf = log(TOTAL_DOC_COUNT / docCount)

                //    //tf-idf = tf*idf
                //})
                .ForEachOutput(f =>
                {
                    Console.WriteLine(f);
                    return new int[] { };
                })
                //.ForEachOutput(f =>
                //{

                //})
                .Process();

            // TODO: Node 2
            // TODO: input - path to a file, output - array of words 
        }

        private static string GenerateFileName(string dirName, FileInfo f, int fileIndex)
        {
            return Path.Combine(dirName, Path.GetFileNameWithoutExtension(f.Name) + $"_{fileIndex}.txt");
        }
    }
}