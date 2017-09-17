using System.Collections.Generic;
using System.Drawing;
using PipelinesLib.Jobs;
using Zavolokas.ParallelComputing.Jobs;

namespace PipelinesLib.Tasks
{
    public class MarkupBitmapTask : JobProducerTask<Bitmap, BitmapWithMarkup>
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _priority;

        public MarkupBitmapTask(int width, int height, List<IJob> jobs, IJobDataStorage storage, int priority)
            : base(jobs, storage)
        {
            _width = width;
            _height = height;
            _priority = priority;
        }

        protected override Job<Bitmap, BitmapWithMarkup> CreateJob()
        {
            return new MarkupBitmapJob(JobDataStorage, _priority, _width, _height);
        }

        public override BitmapWithMarkup[] Process(Bitmap bitmap)
        {
            var result = new List<BitmapWithMarkup>();
            int wp = bitmap.Width / _width;
            int hp = bitmap.Height / _height;

            for (int y = 0; y < hp; y++)
            {
                for (int x = 0; x < wp; x++)
                {
                    var arg = new BitmapWithMarkup { Bmp = bitmap, X = (int)(x * 0.5 * _width), Y = (int)(y * 0.5 * _height), Width = _width, Height = _height };
                    result.Add(arg);
                }
            }

            return result.ToArray();
        }
    }
}