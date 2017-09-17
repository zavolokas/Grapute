using System.Collections.Generic;
using System.Drawing;
using Zavolokas.ParallelComputing.Jobs;

namespace PipelinesLib.Jobs
{
    public class MarkupBitmapJob : Job<Bitmap, BitmapWithMarkup>
    {
        public int Width;
        public int Height;

        public MarkupBitmapJob(IJobDataStorage storage, int priority, int width, int height) 
            : base(storage, priority)
        {
            Width = width;
            Height = height;
        }

        public MarkupBitmapJob(IJobDataStorage storage, int priority)
            : base(storage, priority)
        {
        }

        protected override BitmapWithMarkup[] Process(Bitmap bitmap)
        {
            var result = new List<BitmapWithMarkup>();
            int wp = bitmap.Width / Width;
            int hp = bitmap.Height / Height;

            for (int y = 0; y < hp; y++)
            {
                for (int x = 0; x < wp; x++)
                {
                    var arg = new BitmapWithMarkup { Bmp = bitmap, X = (int)(x * 0.5 * Width), Y = (int)(y * 0.5 * Height), Width = Width, Height = Height };
                    result.Add(arg);
                }
            }

            return result.ToArray();
        }
    }
}