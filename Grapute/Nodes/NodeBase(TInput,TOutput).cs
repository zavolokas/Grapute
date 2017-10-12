namespace Grapute
{
    public abstract class NodeBase<TInput, TOutput> : INode, IOutputNodes<TOutput>
    {
        protected IOutputNodes<TInput> NodeInputProvider;
        protected TInput Input;
        public TOutput[] Output { get; protected set; }

        public void Reset()
        {
            Output = null;
        }

        void INode.Process()
        {
            Process();
        }

        public void SetInput(IOutputNodes<TInput> nodeInputProvider)
        {
            NodeInputProvider = nodeInputProvider;
            Input = default(TInput);
        }

        public abstract NodeBase<TInput, TOutput> Process();
    }
}