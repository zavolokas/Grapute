using System.IO;
using System.Linq;
using Grapute;

namespace MapReduce
{
    class Program
    {
        static void Main(string[] args)
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
                .ForArray(new SplitNode())
                .ForEachOutput(new Map1Node())
                .ForEachOutput(new SortNode())
                .ForEachOutput(new ReduceNode())
                .CollectAllOutputsToOneArray()
                .ForArray(new MergeNode())
                .ForEachOutput(new Reduce2Node())
                .Process();
        }
    }
}