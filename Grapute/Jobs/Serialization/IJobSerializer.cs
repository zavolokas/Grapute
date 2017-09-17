using System.IO;

namespace Grapute.Jobs.Serialization
{
    public interface IJobSerializer
    {
        void SaveToStream(IJob job, Stream stream);
    }
}