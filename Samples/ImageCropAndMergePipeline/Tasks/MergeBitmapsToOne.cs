using System.Drawing;
using System.Linq;
using Grapute;

namespace ChainPipelinesSample.Tasks
{
    public class MergeBitmapsToOne : Task<Bitmap[], Bitmap>
    {
        private readonly int _cols;

        public MergeBitmapsToOne(int cols)
        {
            _cols = cols;
        }

        protected override Bitmap[] Process(Bitmap[] input)
        {
            int rows = input.Length / _cols + 1;
            int maxWidth = input.Max(i => i.Width);
            int maxHeight = input.Max(i => i.Height);

            int resultWidth = maxWidth * _cols;
            int resultHeigth = maxHeight * rows;

            Bitmap result = new Bitmap(resultWidth, resultHeigth);

            using (var g = Graphics.FromImage(result))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    int x = (i % _cols) * maxWidth;
                    int y = (i / _cols) * maxHeight;

                    g.DrawImage(input[i], x, y);
                }
            }

            return new[] { result };
        }
    }
}