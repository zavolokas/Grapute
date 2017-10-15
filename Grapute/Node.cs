using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grapute
{
    public abstract class Node<TInput, TOutput> : NodeBase<TInput, TOutput>
    {
        public override INode<TOutput> Process()
        {
            var inputs = new List<TInput>();

            if (NodeInputProvider != null)
            {
                NodeInputProvider.Process();
                inputs.AddRange(NodeInputProvider.Output);
            }
            else if (Input != null)
            {
                inputs.Add(Input);
            }

            //process inputs and put result to the Output

            var outputs = new ConcurrentBag<TOutput>();
            Parallel.ForEach(Partitioner.Create(0, inputs.Count), range =>
            {
                for (var i = range.Item1; i < range.Item2; i++)
                {
                    var output = Process(inputs[i]);
                    for (var j = 0; j < output.Length; j++)
                    {
                        outputs.Add(output[j]);
                    }
                }
            });


            Output = outputs.ToArray();
            return this;
        }

        public SinkNode<TOutput> CollectAllOutputsToOneArray()
        {
            var node = new SinkNode<TOutput>();
            node.SetInput(this);
            return node;
        }

        public void SetInput(TInput input)
        {
            Input = input;
            NodeInputProvider = null;
        }

        protected abstract TOutput[] Process(TInput input);
    }
}