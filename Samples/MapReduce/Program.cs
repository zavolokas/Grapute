using System.IO;
using System.Linq;
using Grapute;

namespace MapReduce
{
    internal class Program
    {
        private static void Main()
        {
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
                .ForArray(new SplitFilesIntoChunks())
                .ForEachOutput(new InitTermCount())
                .ForEachOutput(new SortTokens())
                .ForEachOutput(new ReduceToUniqueTokens())
                .CollectAllOutputsToOneArray()
                .ForArray(new MergeSort())
                .ForEachOutput(new ReduceToUniqueTokens2())
                .Process();
        }
    }
}