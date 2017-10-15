using System;
using System.Collections.Generic;

namespace Grapute
{
    public class MergeNode<T> : NodeBase<T, T[]>
    {
        public override IOutputNodes<T[]> Process()
        {
            var inputs = new List<T>();

            if (NodeInputProvider != null)
            {
                NodeInputProvider.Process();
                inputs.AddRange(NodeInputProvider.Output);
            }
            else if (Input != null)
            {
                inputs.Add(Input);
            }

            Output = new T[1][];
            Output[0] = inputs.ToArray();
            return this;
        }

        public IForEach<T[], TNewOutput> ForArray<TNewOutput>(CommonNode<T[], TNewOutput> processOutputsNode)
        {
            processOutputsNode.SetInput(this);
            return processOutputsNode;
        }

        public IForEach<T[], TNewOutput> ForArray<TNewOutput>(Func<T[], TNewOutput[]> processOutputsFunc)
        {
            var node = new FuncNode<T[], TNewOutput>(processOutputsFunc);
            node.SetInput(this);
            return node;
        }
    }
}