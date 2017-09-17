using System.Drawing;
using System.Linq;
using Zavolokas.ParallelComputing.Jobs;

namespace PipelinesLib.Jobs
{
    public class MergeBitmapsJob : Job<Bitmap[], Bitmap>
    {
        /// <summary>
        /// Max amount of colums to put bitmaps in there.
        /// </summary>
        /// <value>
        /// The columns amount.
        /// </value>
        public int Cols { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeBitmapsJob"/> class.
        /// </summary>
        /// <param name="storage">The data storage.</param>
        /// <param name="priority">The priority of the job.</param>
        public MergeBitmapsJob(IJobDataStorage storage, int priority)
            : base(storage, priority)
        {
        }

        /// <summary>
        /// Merges the input bitmaps into a one bitmap.
        /// </summary>
        /// <param name="input">The input bitmaps.</param>
        /// <returns></returns>
        protected override Bitmap[] Process(Bitmap[] input)
        {
            int rows = input.Length / Cols + 1;
            int maxWidth = input.Max(i => i.Width);
            int maxHeight = input.Max(i => i.Height);

            int resultWidth = maxWidth * Cols;
            int resultHeigth = maxHeight * rows;

            Bitmap result = new Bitmap(resultWidth, resultHeigth);

            using (var g = Graphics.FromImage(result))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    int x = (i % Cols) * maxWidth;
                    int y = (i / Cols) * maxHeight;

                    g.DrawImage(input[i], x, y);
                }
            }

            return new []{result};
        }
    }
}