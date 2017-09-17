namespace Grapute.Jobs
{
    public interface IJobSaver
    {
        void Save(IJob job);
    }
}