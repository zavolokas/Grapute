
namespace Grapute.Jobs
{
    public interface IJobRestorer
    {
        IJob GetHighestPriorityJob();
    }
}