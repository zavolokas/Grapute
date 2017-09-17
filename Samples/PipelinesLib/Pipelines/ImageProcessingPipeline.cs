using System.Collections.Generic;
using System.Drawing;
using Zavolokas.ParallelComputing.Jobs;
using Zavolokas.ParallelComputing.Tasks;

namespace PipelinesLib.Pipelines
{
    /// <summary>
    /// The pipeline that processes an image.
    /// </summary>
    /// <seealso cref="Task{T, TF}" />
    public class ImageProcessingPipeline : Task<JobData<Bitmap>, JobData<Bitmap>>
    {
        private readonly IJobDataStorage _storage;
        private List<IJob> _jobs;

        public IJob[] GetJobs()
        {
            return _jobs.ToArray();
        }

        public ImageProcessingPipeline(IJobDataStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Processes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        protected override JobData<Bitmap>[] Process(JobData<Bitmap> input)
        {
            _jobs = new List<IJob>();

            int priority = 0;
            var pipeline = new ImageCropMergePipeline(new Size(640, 512), new Size(320, 256), 4, _jobs, _storage, priority);
            var pipeline2 = new ImageCropMergePipeline(new Size(320, 256), new Size(160, 128), 8, _jobs, _storage, priority + 10);

            return pipeline
                .SetInput(input)
                .ForEachOutput(pipeline2)
                .Process()
                .Output;
        }
    }
}