using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PipelinesLib.Jobs;
using Zavolokas.ParallelComputing.Jobs;

namespace PipelinesLib.Tasks
{
    public class MergeBitmapsToOneTask : MergeJobProducerTask<Bitmap, Bitmap>
    {
        private readonly int _cols;
        private readonly int _priority;

        public MergeBitmapsToOneTask(int cols, List<IJob> jobs, IJobDataStorage storage, int priority)
            : base(jobs, storage)
        {
            _cols = cols;
            _priority = priority;
        }

        protected override Bitmap[] Process(Bitmap[] bitmaps)
        {
            // Calculate the Width and Height for the fake output
            int rows = bitmaps.Length / _cols + 1;
            int maxWidth = bitmaps.Max(b => b.Width);
            int maxHeight = bitmaps.Max(b => b.Height);

            int resultWidth = maxWidth * _cols;
            int resultHeigth = maxHeight * rows;

            //I don't like the fact that I need to create this fake result here.
            var result = new Bitmap(resultWidth, resultHeigth);
            return new[] { result };
        }

        protected override Job<Bitmap[], Bitmap> CreateJob()
        {
            var job = new MergeBitmapsJob(JobDataStorage, _priority);

            // assign fields that are required by the job 
            job.Cols = _cols;

            return job;
        }
    }
}