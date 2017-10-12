using System.Drawing;
using System.Linq;

namespace ImageShufflePipeline
{
    internal class ImageProcessingFunctions
    {
        public BitmapRegion[] DivideIn4Regions(Bitmap input)
        {
            int wp = 2;
            int hp = 2;

            var result = new BitmapRegion[wp * hp];

            int width = input.Width / wp;
            int height = input.Height / hp;

            for (var y = 0; y < hp; y++)
            {
                for (var x = 0; x < wp; x++)
                {
                    var arg = new BitmapRegion
                    {
                        Bmp = input,
                        X = x * width,
                        Y = y * height,
                        Width = width,
                        Height = height
                    };
                    result[y * wp + x] = arg;
                }
            }

            return result;
        }

        public Bitmap ExtractToNewBitmap(BitmapRegion input)
        {
            var result = new Bitmap(input.Width, input.Height);
            var bmp = input.Bmp;
            using (var graphics = Graphics.FromImage(result))
            {
                var destRect = new Rectangle(0, 0, result.Width, result.Height);
                var srcRect = new Rectangle(input.X, input.Y, input.Width, input.Height);
                graphics.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
            }

            return result;
        }

        public Bitmap MergeRegions(Bitmap[] input)
        {
            int cols = 7;
            int rows = input.Length / cols + 1;
            int maxWidth = input.Max(i => i.Width);
            int maxHeight = input.Max(i => i.Height);

            int resultWidth = maxWidth * cols;
            int resultHeigth = maxHeight * rows;

            Bitmap result = new Bitmap(resultWidth, resultHeigth);

            using (var g = Graphics.FromImage(result))
            {
                for (int i = 0; i < input.Length; i++)
                {
                    int x = (i % cols) * maxWidth;
                    int y = (i / cols) * maxHeight;

                    g.DrawImage(input[i], x, y);
                }
            }

            return result;
        }
    }
}