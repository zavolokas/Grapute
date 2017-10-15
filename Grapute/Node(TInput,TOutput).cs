using System;
using System.Collections.Generic;

namespace Grapute
{
    //public abstract class Node<TInput, TOutput> : SingleNode<TInput, TOutput>
    //{
    //    public FuncNode<TOutput, TNewOutput> ForEachOutput<TNewOutput>(Func<TOutput, TNewOutput[]> processOutputsFunc)
    //    {
    //        var node = new FuncNode<TOutput,TNewOutput>(processOutputsFunc);
    //        node.SetInput(this);
    //        return node;
    //    }

    //    public Node<TOutput, TNewOutput> ForEachOutput<TNewOutput>(Node<TOutput, TNewOutput> processOutputsNode)
    //    {
    //        processOutputsNode.SetInput(this);
    //        return processOutputsNode;
    //    }

    //    public Node<TInput, TOutput> SetInput(TInput input)
    //    {
    //        Input = input;
    //        NodeInputProvider = null;
    //        return this;
    //    }
    //}

    
}