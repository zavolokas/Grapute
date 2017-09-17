using System;
using System.Diagnostics;
using System.IO;
using PipelinesLib;
using Zavolokas.ParallelComputing.Jobs;
using Zavolokas.ParallelComputing.Jobs.Serialization;
using Zavolokas.ParallelComputing.Jobs.Storage;

namespace PipelineExecutor
{
    class Program
    {
        static void Main(string[] args)
        {
            const string basePath = "..\\..\\..\\storage";

            IJobDataStorage dataStorage = new FileSystemJobDataStorage(basePath);
            IJobDeserializer jobDeserializer = new ImgProcJobBinaryDeserializer(dataStorage);
            IJobRestorer jobRestorer = new FileSystemJobRestorer(basePath, jobDeserializer);

            DataIdentifyer[] lastOutput = null;
            IJob job = jobRestorer.GetHighestPriorityJob();
            int jobNumber = 0;
            while (job != null)
            {
                jobNumber++;
                Console.WriteLine($"Job #{jobNumber} processing...");
                job.Init(dataStorage);
                job.Process();
                lastOutput = job.Outputs;
                job = jobRestorer.GetHighestPriorityJob();
            }

            Console.WriteLine("Job processing finished.");

            string filename = Path.Combine(basePath, lastOutput[0].Id);
            Process.Start(filename);
        }
    }
}
