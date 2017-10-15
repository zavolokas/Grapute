using System;

namespace Grapute
{
    public static class NodeExtensions
    {
        public static Node<TOutput, TNewOutput> ForEachOutput<TInput, TOutput, TNewOutput>(this Node<TInput, TOutput> thisNode, Func<TOutput, TNewOutput[]> processOutputsFunc)
        {
            var node = new FuncNode<TOutput, TNewOutput>(processOutputsFunc);
            node.SetInput(thisNode);
            return node;
        }

        public static Node<TOutput, TNewOutput> ForEachOutput<TInput, TOutput, TNewOutput>(this Node<TInput, TOutput> thisNode, Node<TOutput, TNewOutput> processOutputsNode)
        {
            processOutputsNode.SetInput(thisNode);
            return processOutputsNode;
        }

        public static Node<T[], TNewOutput> ForArray<T,TNewOutput>(this SinkNode<T> thisNode, Node<T[], TNewOutput> processOutputsNode)
        {
            processOutputsNode.SetInput(thisNode);
            return processOutputsNode;
        }

        public static Node<T[], TNewOutput> ForArray<T, TNewOutput>(this SinkNode<T> thisNode, Func<T[], TNewOutput[]> processOutputsFunc)
        {
            var node = new FuncNode<T[], TNewOutput>(processOutputsFunc);
            node.SetInput(thisNode);
            return node;
        }
    }
}