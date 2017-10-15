using System;

namespace Grapute
{
    public abstract class CommonNode<TInput, TOutput> : SingleNode<TInput, TOutput>, IForEach<TInput, TOutput>
    {
        public IForEach<TOutput, TNewOutput> ForEachOutput<TNewOutput>(Func<TOutput, TNewOutput[]> processOutputsFunc)
        {
            var node = new FuncNode<TOutput, TNewOutput>(processOutputsFunc);
            node.SetInput(this);
            return node;
        }

        public IForEach<TOutput, TNewOutput> ForEachOutput<TNewOutput>(CommonNode<TOutput, TNewOutput> processOutputsNode)
        {
            processOutputsNode.SetInput(this);
            return processOutputsNode;
        }

        public void SetInput(TInput input)
        {
            Input = input;
            NodeInputProvider = null;
        }
    }
}