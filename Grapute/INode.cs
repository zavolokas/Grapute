
namespace Grapute
{
    public interface INode<out T>
    {
        void Process();
        T[] Output { get; }
    }
}
