using System;

namespace Grapute
{
    public sealed class FuncNode<TInput, TOutput> : Node<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput[]> _processFunction;

        public FuncNode(Func<TInput, TOutput[]> processFunction)
        {
            _processFunction = processFunction;
        }

        protected override TOutput[] Process(TInput input)
        {
            return _processFunction(input);
        }
    }
}