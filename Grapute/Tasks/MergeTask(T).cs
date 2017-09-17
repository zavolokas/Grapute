using System.Collections.Generic;

namespace Grapute
{
    public class MergeTask<T> : TaskBase<T, T[]>
    {
        public override TaskBase<T, T[]> Process()
        {
            if (IsFinished)
                return this;

            var inputs = new List<T>();

            if (TaskInputProvider != null && !TaskInputProvider.IsFinished)
            {
                TaskInputProvider.Process();
                inputs.AddRange(TaskInputProvider.Output);
            }
            else if (Input != null)
            {
                inputs.Add(Input);
            }

            Output = new T[1][];
            Output[0] = inputs.ToArray();
            IsFinished = true;
            return this;
        }

        public Task<T[], TNewOutput> ForArray<TNewOutput>(Task<T[], TNewOutput> processOutputsTask)
        {
            processOutputsTask.SetInput(this);
            return processOutputsTask;
        }
    }
}