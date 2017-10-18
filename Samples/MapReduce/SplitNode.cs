using System;
using System.Collections.Generic;
using System.IO;
using Grapute;

namespace MapReduce
{
    class SplitNode: Node<FileInfo[], FileInfo>
    {
        private static string GenerateFileName(string dirName, FileInfo f, int fileIndex)
        {
            return Path.Combine(dirName, Path.GetFileNameWithoutExtension(f.Name) + $"_{fileIndex}.txt");
        }

        protected override FileInfo[] Process(FileInfo[] fileInfos)
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
        }
    }
}