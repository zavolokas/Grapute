using System;
using System.Collections.Generic;

namespace Grapute
{
    public class Node<TInput, TOutput> : NodeBase<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput[]> _processFunction;
        public Node(Func<TInput, TOutput[]> processFunction)
        {
            _processFunction = processFunction;
        }

        public override NodeBase<TInput, TOutput> Process()
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

        protected TOutput[] Process(TInput input)
        {
            return _processFunction(input);
        }

        public Node<TOutput, TNewOutput> ForEachOutput<TNewOutput>(Func<TOutput, TNewOutput[]> processOutputsFunc)
        {
            var node = new Node<TOutput,TNewOutput>(processOutputsFunc);
            node.SetInput(this);
            return node;
        }

        public Node<TOutput, TNewOutput> ForEachOutput<TNewOutput>(Node<TOutput, TNewOutput> processOutputsNode)
        {
            processOutputsNode.SetInput(this);
            return processOutputsNode;
        }

        public MergeNode<TOutput> CollectAllOutputsToOneArray()
        {
            var node = new MergeNode<TOutput>();
            node.SetInput(this);
            return node;
        }

        public Node<TInput, TOutput> SetInput(TInput input)
        {
            Input = input;
            NodeInputProvider = null;
            return this;
        }
    }
}