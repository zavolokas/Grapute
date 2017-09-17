using System;
using System.Collections.Generic;

namespace Grapute
{
    public abstract class Task<TInput, TOutput> : TaskBase<TInput, TOutput>
    {
        public override TaskBase<TInput, TOutput> Process()
        {
            if (IsFinished)
                return this;

            var inputs = new List<TInput>();

            if (TaskInputProvider != null && !TaskInputProvider.IsFinished)
            {
                TaskInputProvider.Process();
                inputs.AddRange(TaskInputProvider.Output);
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
            IsFinished = true;
            return this;
        }

        public Task<TOutput, TNewOutput> ForEachOutput<TNewOutput>(Task<TOutput, TNewOutput> processOutputsTask)
        {
            processOutputsTask.SetInput(this);
            return processOutputsTask;
        }

        public MergeTask<TOutput> CollectAllOutputsToOneArray()
        {
            var task = new MergeTask<TOutput>();
            task.SetInput(this);
            return task;
        }

        public Task<TInput, TOutput> SetInput(TInput input)
        {
            Input = input;
            TaskInputProvider = null;
            return this;
        }

        protected abstract TOutput[] Process(TInput input);
    }
}