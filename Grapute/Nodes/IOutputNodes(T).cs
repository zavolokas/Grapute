namespace Grapute
{
    public interface IOutputNodes<out T> : INode
    {
        T[] Output { get; } 
    }
}