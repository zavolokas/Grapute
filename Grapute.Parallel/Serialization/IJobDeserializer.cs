using System.IO;

namespace Grapute.Jobs.Serialization
{
    public interface IJobDeserializer
    {
        IJob FromStream(Stream stream);
    }
}