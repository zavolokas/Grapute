using System.Drawing;
using Grapute.Jobs;

namespace PipelinesLib.Jobs
{
    /// <summary>
    /// The job that extracts a marked part of a bitmap to a new bitmap.
    /// </summary>
    /// <seealso cref="Job{TInput,TOutput}" />
    public class ExtractMarkedBitmapPartJob : Job<BitmapWithMarkup, Bitmap>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractMarkedBitmapPartJob"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="priority">The priority of the job.</param>
        public ExtractMarkedBitmapPartJob(IJobDataStorage storage, int priority)
            : base(storage, priority)
        {
        }

        /// <summary>
        /// Runs the job.
        /// </summary>
        /// <param name="input">The markup.</param>
        /// <returns></returns>
        protected override Bitmap[] Process(BitmapWithMarkup input)
        {
            var result = new Bitmap(input.Width, input.Height);
            using (var graphics = Graphics.FromImage(result))
            {
                var destRect = new Rectangle(0, 0, result.Width, result.Height);
                var srcRect = new Rectangle(input.X, input.Y, input.Width, input.Height);
                graphics.DrawImage(input.Bmp, destRect, srcRect, GraphicsUnit.Pixel);
            }

            return new []{result};
        }
    }
}