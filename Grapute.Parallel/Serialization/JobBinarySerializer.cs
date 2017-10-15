using System.IO;

namespace Grapute.Jobs.Serialization
{
    public abstract class JobBinarySerializer
    {
        public void SaveToStream(IJob job, Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                WriteJob(job, writer);
            }
        }

        protected abstract void WriteJob(IJob job, BinaryWriter writer);

        protected void WriteBase(IJob job, BinaryWriter w)
        {
            w.Write(job.Priority);
            w.Write(job.RequiredComputeResources);
            w.Write(job.Input.Id);
            w.Write(job.Outputs.Length);
            for (int i = 0; i < job.Outputs.Length; i++)
            {
                w.Write(job.Outputs[i].Id);
            }
        }
    }
}