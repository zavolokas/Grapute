using System.Collections.Generic;

namespace Grapute
{
    public abstract class SingleNode<TInput, TOutput> : NodeBase<TInput, TOutput>
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
            var outputs = new List<TOutput>();
            foreach (var input in inputs)
            {
                var output = Process(input);
                outputs.AddRange(output);
            }

            Output = outputs.ToArray();
            return this;
        }

        public MergeNode<TOutput> CollectAllOutputsToOneArray()
        {
            var node = new MergeNode<TOutput>();
            node.SetInput(this);
            return node;
        }

        protected abstract TOutput[] Process(TInput input);
    }
}