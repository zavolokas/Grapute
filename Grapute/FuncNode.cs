using System;

namespace Grapute
{
    public sealed class FuncNode<TInput, TOutput> : SingleNode<TInput, TOutput>, IForEach<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput[]> _processFunction = null;

        public FuncNode(Func<TInput, TOutput[]> processFunction)
        {
            _processFunction = processFunction;
        }

        protected override TOutput[] Process(TInput input)
        {
            return _processFunction(input);
        }

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