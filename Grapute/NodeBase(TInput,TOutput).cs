using System;

namespace Grapute
{
    public abstract class NodeBase<TInput, TOutput> : INode<TOutput>
    {
        protected INode<TInput> NodeInputProvider;
        protected TInput Input;
        public TOutput[] Output { get; protected set; }

        public void Reset()
        {
            Output = null;
        }

        void INode<TOutput>.Process()
        {
            Process();
        }

        public void SetInput(INode<TInput> nodeInputProvider)
        {
            NodeInputProvider = nodeInputProvider;
            Input = default(TInput);
        }

        public abstract INode<TOutput> Process();
    }
}