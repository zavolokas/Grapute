using System;
using System.Text;
using Grapute;

namespace SampleComputatuions
{
    class Program
    {
        static void Main(string[] args)
        {
            var startNode = new Node<int, int>(x =>
            {
                Console.WriteLine("Pow 2 and double");
                return new[] { x * x, x * x };
            });

            var pipeline = startNode
                    .ForEachOutput(x =>
                        {
                            Console.WriteLine("Double and add 1 and 2");
                            return new[] { x + 1, x + 2 };
                        })
                    //.ForEachOutput(x => new[] { x + 3, x + 4 })
                    //.ForEachOutput(x => new[] {x + 5, x + 6})*
                    //.ForEachOutput(x => new[] {x + 1})
                    //.ForEachOutput(x => new[] {x + 4})
                    .CollectAllOutputsToOneArray()
                    .ForArray(x =>
                    {
                        Console.WriteLine("Gather to one string");
                        var sb = new StringBuilder();
                        for (int i = 0; i < x.Length; i++)
                        {
                            sb.AppendFormat("{0} HQ ", x[i]);
                        }
                        return new[] { sb.ToString() };
                    });


            startNode.SetInput(5);

            pipeline.Process();
            var result1 = pipeline.Output[0];
            Console.WriteLine(result1);

            startNode.SetInput(10);
            pipeline.Process();
            var result2 = pipeline.Output[0];
            Console.WriteLine(result2);
        }
    }
}
