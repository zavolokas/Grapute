using System;
using System.IO;
using System.Linq;
using Grapute.Jobs.Serialization;

namespace Grapute.Jobs.Storage
{
    public class FileSystemJobRestorer : IJobRestorer
    {
        private readonly string _basePath;
        private readonly IJobDeserializer _jobDeserializer;

        public FileSystemJobRestorer(string basePath, IJobDeserializer jobDeserializer)
        {
            if (basePath == null)
                throw new ArgumentNullException(nameof(basePath), "Base path can not be null");

            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException($"Directory '{basePath}' is not found.");

            _basePath = basePath;
            _jobDeserializer = jobDeserializer;
        }

        public IJob GetHighestPriorityJob()
        {
            //get a list of saved jobs in the directory
            var jobFiles = Directory.GetFiles(_basePath, "job_*.dat");

            if (jobFiles == null || jobFiles.Length == 0) return null;

            //take one with the highest priority
            var jobInofs = new Tuple<string,int>[jobFiles.Length];
            for (int i = 0; i < jobFiles.Length; i++)
            {
                var jobFile = jobFiles[i];
                var prioStart = jobFile.IndexOf("_", StringComparison.Ordinal);
                var prioLen = jobFile.Substring(prioStart + 1, jobFile.Length - prioStart - 1)
                    .IndexOf("_", StringComparison.Ordinal);

                var priority = int.Parse(jobFile.Substring(prioStart + 1, prioLen));
                jobInofs[i] = new Tuple<string, int>(jobFile, priority);
            }

            //todo: there should be some locking job mechanism
            // to prevent an access to one job from multiple apps
            // for now we simply pick a job.
            var priorityJob = jobInofs.Aggregate((curMin, x) => (curMin.Item2 > x.Item2 ? x : curMin)).Item1;

            // deserialize it and return.
            IJob job;
            using (var stream = File.OpenRead(priorityJob))
            {
                job = _jobDeserializer.FromStream(stream);
            }

            // remove the job from storage
            File.Delete(priorityJob);

            return job;
        }
    }
}