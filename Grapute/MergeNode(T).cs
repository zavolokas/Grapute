using System;
using System.Collections.Generic;

namespace Grapute
{
    public class MergeNode<T> : NodeBase<T, T[]>
    {
        public override NodeBase<T, T[]> Process()
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

        public Node<T[], TNewOutput> ForArray<TNewOutput>(Node<T[], TNewOutput> processOutputsNode)
        {
            processOutputsNode.SetInput(this);
            return processOutputsNode;
        }

        public Node<T[], TNewOutput> ForArray<TNewOutput>(Func<T[], TNewOutput[]> processOutputsFunc)
        {
            var node = new Node<T[], TNewOutput>(processOutputsFunc);
            node.SetInput(this);
            return node;
        }
    }
}