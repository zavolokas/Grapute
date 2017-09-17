
namespace Grapute
{
    public interface ITask
    {
        void Process();
        bool IsFinished { get; }
    }
}
