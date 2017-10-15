using System.Collections.Generic;

namespace Grapute
{
    public class SinkNode<T> : NodeBase<T, T[]>
    {
        public override INode<T[]> Process()
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
    }
}