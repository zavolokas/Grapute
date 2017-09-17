using System.Collections.Generic;
using System.Drawing;
using PipelinesLib.Tasks;
using Zavolokas.ParallelComputing.Jobs;
using Zavolokas.ParallelComputing.Tasks;

namespace PipelinesLib.Pipelines
{
    /// <summary>
    /// The pipeline of tasks that splits and merges an image.
    /// </summary>
    /// <seealso cref="Task{T, TF}" />
    internal class ImageCropMergePipeline : Task<JobData<Bitmap>, JobData<Bitmap>>
    {
        private readonly Size _size1;
        private readonly Size _size2;
        private readonly int _cols;
        private readonly List<IJob> _jobs;
        private readonly IJobDataStorage _storage;
        private  int _priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageCropMergePipeline"/> class.
        /// </summary>
        /// <param name="size1">The size to split initial image.</param>
        /// <param name="size2">The size to split the images second time.</param>
        /// <param name="cols">The amount of columns to merge images into.</param>
        /// <param name="jobs">The jobs collection to collect the jobs.</param>
        /// <param name="storage">The job data storage.</param>
        /// <param name="priority">The initial priority of the pipeline.</param>
        public ImageCropMergePipeline(Size size1, Size size2, int cols, List<IJob> jobs, IJobDataStorage storage, int priority)
        {
            _size1 = size1;
            _size2 = size2;
            _cols = cols;
            _jobs = jobs;
            _storage = storage;
            _priority = priority;
        }

        /// <summary>
        /// Processes the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
        protected override JobData<Bitmap>[] Process(JobData<Bitmap> bitmap)
        {
            return new MarkupBitmapTask(_size1.Width, _size1.Height, _jobs, _storage, _priority++)
                .SetInput(bitmap)
                .ForEachOutput(new ExtractBitmapTask(_jobs, _storage, _priority++))
                .ForEachOutput(new MarkupBitmapTask(_size2.Width, _size2.Height, _jobs, _storage, _priority++))
                .ForEachOutput(new ExtractBitmapTask(_jobs, _storage, _priority++))
                .CollectAllOutputsToOneArray()
                .ForArray(new MergeBitmapsToOneTask(_cols, _jobs, _storage, _priority++))
                .Process()
                .Output;
        }
    }
}