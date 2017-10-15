using System;

namespace Grapute
{
    public interface IForEach<in TInput, TOutput>
    {
        IForEach<TOutput, TNewOutput> ForEachOutput<TNewOutput>(Func<TOutput, TNewOutput[]> processOutputsFunc);
        IForEach<TOutput, TNewOutput> ForEachOutput<TNewOutput>(CommonNode<TOutput, TNewOutput> processOutputsNode);

        void SetInput(TInput input);
        MergeNode<TOutput> CollectAllOutputsToOneArray();

        IOutputNodes<TOutput> Process();
    }
}