using System;
using System.Drawing;
using System.IO;
using Grapute.Jobs;
using Grapute.Jobs.Storage;
using PipelinesLib;
using PipelinesLib.Pipelines;

namespace PipelineCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            const string basePath = "..\\..\\..\\images";

            // this is our input bitmap.
            string filePath = Path.Combine(basePath, "image1.jpg");
            var bitmap = new Bitmap(filePath);

            // Input should be placed into the storage so 
            // that jobs could reach it.
            // Create a storage and save the input there.
            var storage = new FileSystemJobDataStorage(basePath);
            var id = storage.GenerateDataIdentifyer(bitmap);
            storage.SaveData(bitmap, id);

            // Pipeline that creates the set of jobs needs 
            // the input bitmap as well. That is why we pack 
            // everything (id and bitmap) in a structure that used
            // by the pipeline.
            var input = new JobData<Bitmap>
            {
                Data = bitmap,
                Id = id
            };

            // Init the pipeline
            var pipeline = new ImageProcessingPipeline(storage);
            pipeline.SetInput(input);

            // Pipeline builds an unique set of jobs 
            // that are created especially for this particular input.
            pipeline.Process();

            // get the jobs and save them to use from 
            // another application to demonstrate how these two
            // processes are independent.
            var jobs = pipeline.GetJobs();

            IJobSaver jobSaver = new FileSystemJobSaver(basePath, new ImgProcJobBinarySerializer());
            foreach (var job in jobs)
            {
                jobSaver.Save(job);
            }

            Console.WriteLine($"{jobs.Length} jobs were created and saved to {basePath}");
        }
    }
}
