namespace Grapute
{
    public interface IOutputTasks<out T> : ITask
    {
        T[] Output { get; } 
    }
}