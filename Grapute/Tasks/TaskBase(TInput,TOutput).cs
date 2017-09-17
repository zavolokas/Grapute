namespace Grapute
{
    public abstract class TaskBase<TInput, TOutput> : ITask, IOutputTasks<TOutput>
    {
        protected IOutputTasks<TInput> TaskInputProvider;
        protected TInput Input;
        public TOutput[] Output { get; protected set; }

        public bool IsFinished { get; protected set; }

        public void Reset()
        {
            Output = null;
            IsFinished = false;
        }

        void ITask.Process()
        {
            Process();
        }

        public void SetInput(IOutputTasks<TInput> taskInputProvider)
        {
            TaskInputProvider = taskInputProvider;
            Input = default(TInput);
        }

        public abstract TaskBase<TInput, TOutput> Process();
    }
}