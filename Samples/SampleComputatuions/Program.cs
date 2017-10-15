using System;
using System.Text;
using Grapute;

namespace SampleComputatuions
{

    class SumNode : CommonNode<int, int>
    {
        protected override int[] Process(int input)
        {
            return new[] { input + 0 };
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var startNode = new FuncNode<int, int>(x =>
            {
                Console.WriteLine("Pow 2 and double");
                return new[] { x * x, x * x };
            });

            var sumNode = new SumNode();

            var pipeline = startNode
                    .ForEachOutput(x =>
                        {
                            Console.WriteLine("Double and add 1 and 2");
                            return new[] { x + 1, x + 2 };
                        })
                    .ForEachOutput(sumNode)
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

            var result1 = pipeline.Process()
                .Output[0];
            Console.WriteLine(result1);

            startNode.SetInput(10);
            pipeline.Process();
            var result2 = pipeline.Process()
                .Output[0];
            Console.WriteLine(result2);
        }
    }
}
