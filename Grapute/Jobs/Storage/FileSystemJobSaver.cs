using System;
using System.IO;
using Grapute.Jobs.Serialization;

namespace Grapute.Jobs.Storage
{
    public class FileSystemJobSaver : IJobSaver
    {
        private readonly string _basePath;
        private readonly IJobSerializer _jobSerializer;

        public FileSystemJobSaver(string basePath, IJobSerializer jobSerializer)
        {
            if (basePath == null)
                throw new ArgumentNullException(nameof(basePath), "Base path can not be null");

            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException($"Directory '{basePath}' is not found.");

            _basePath = basePath;
            _jobSerializer = jobSerializer;
        }

        public void Save(IJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            string fileName = $"job_{job.Priority}_{Guid.NewGuid()}.dat";

            using (var stream = File.OpenWrite(Path.Combine(_basePath, fileName)))
            {
                _jobSerializer.SaveToStream(job, stream);
            }
        }
    }
}