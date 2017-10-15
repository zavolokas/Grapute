using System.IO;

namespace Grapute.Jobs.Serialization
{
    public abstract class JobBinaryDeserializer : IJobDeserializer
    {
        protected JobBinaryDeserializer(IJobDataStorage jobDataStorage)
        {
            JobDataStorage = jobDataStorage;
        }

        protected IJobDataStorage JobDataStorage { get; }

        public IJob FromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                return ReadJob(reader);
            }
        }

        protected abstract IJob ReadJob(BinaryReader reader);

        protected void ReadBaseJob(BinaryReader reader, IJob job)
        {
            job.Priority = reader.ReadInt32();
            job.RequiredComputeResources = reader.ReadInt32();
            job.Input = new DataIdentifyer {Id = reader.ReadString()};
            var outputsAmount = reader.ReadInt32();
            job.Outputs = new DataIdentifyer[outputsAmount];
            for (int i = 0; i < job.Outputs.Length; i++)
            {
                job.Outputs[i] = new DataIdentifyer { Id = reader.ReadString() };
            }
        }
    }
}