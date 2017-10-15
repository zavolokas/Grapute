using System.Collections.Generic;
using System.Drawing;
using Grapute.Jobs;
using PipelinesLib.Jobs;

namespace PipelinesLib.Tasks
{
    public class ExtractBitmapTask : JobProducerTask<BitmapWithMarkup, Bitmap>
    {
        private readonly int _priority;

        public ExtractBitmapTask(List<IJob> jobs, IJobDataStorage jobDataStorage, int priority)
            : base(jobs, jobDataStorage)
        {
            _priority = priority;
        }

        protected override Job<BitmapWithMarkup, Bitmap> CreateJob()
        {
            return new ExtractMarkedBitmapPartJob(JobDataStorage, _priority);
        }

        public override Bitmap[] Process(BitmapWithMarkup markup)
        {
            return new[] { new Bitmap(markup.Width, markup.Height) };
        }
    }
}