using System;
using System.Collections.Generic;
using System.IO;
using Grapute;

namespace MapReduce
{
    internal class SplitFilesIntoChunks: Node<FileInfo[], FileInfo>
    {
        private const int PartitionMaxSize = 100_000;

        private static string GenerateFileName(string dirName, FileInfo f, int fileIndex)
        {
            return Path.Combine(dirName, Path.GetFileNameWithoutExtension(f.Name) + $"_{fileIndex}.txt");
        }

        protected override FileInfo[] Process(FileInfo[] fileInfos)
        {
            // split file into many
            var resultFiles = new List<FileInfo>();
            var buffer = new char[1];

            for (var i = 0; i < fileInfos.Length; i++)
            {
                var fileInfo = fileInfos[i];
                var currentFileSize = 0;
                var fileIndex = 0;

                var dirName = Path.Combine(fileInfo.DirectoryName, "mr");
                var fileName = GenerateFileName(dirName, fileInfo, fileIndex);

                var streamWriter = new StreamWriter(fileName);
                resultFiles.Add(new FileInfo(fileName));

                var isLastPunctuation = false;

                using (var streamReader = fileInfo.OpenText())
                {
                    while (!streamReader.EndOfStream)
                    {
                        streamReader.Read(buffer, 0, 1);
                        var currentChar = buffer[0];
                        var isPunctuation = char.IsPunctuation(currentChar) || char.IsWhiteSpace(currentChar);
                        if (char.IsLetter(currentChar) || (isPunctuation && !isLastPunctuation))
                        {
                            currentFileSize++;
                            isLastPunctuation = isPunctuation;

                            if (isPunctuation)
                                currentChar = (char)13;
                            else if (!char.IsLower(currentChar))
                                currentChar = char.ToLower(currentChar);

                            streamWriter.Write(new[] { currentChar }, 0, 1);

                            if (currentFileSize > PartitionMaxSize && isPunctuation)
                            {
                                fileIndex++;
                                fileName = GenerateFileName(dirName, fileInfo, fileIndex);
                                streamWriter.Close();
                                streamWriter.Dispose();
                                streamWriter = new StreamWriter(fileName);
                                resultFiles.Add(new FileInfo(fileName));
                                currentFileSize = 0;
                            }
                        }
                    }
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }

            return resultFiles.ToArray();
        }
    }
}