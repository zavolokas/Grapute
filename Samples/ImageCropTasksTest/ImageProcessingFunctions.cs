using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageCropTasksTest
{
    internal class ImageProcessingFunctions
    {
        public BitmapRegion[] DivideIn4Regions(Bitmap input)
        {
            var result = new List<BitmapRegion>();

            int wp = 2;
            int hp = 2;

            int _width = input.Width / wp;
            int _height = input.Height / hp;

            for (int y = 0; y < hp; y++)
            {
                for (int x = 0; x < wp; x++)
                {
                    var arg = new BitmapRegion() { Bmp = input, X = x * _width, Y = y * _height, Width = _width, Height = _height };
                    result.Add(arg);
                }
            }

            return result.ToArray();
        }

        public Bitmap[] ExtractToNewBitmap(BitmapRegion input)
        {
            var result = new Bitmap(input.Width, input.Height);
            var bmp = input.Bmp;
            using (var graphics = Graphics.FromImage(result))
            {
                var destRect = new Rectangle(0, 0, result.Width, result.Height);
                var srcRect = new Rectangle(input.X, input.Y, input.Width, input.Height);
                graphics.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
            }

            return new[] { result };
        }

        public Bitmap[] MergeRegions(Bitmap[] input)
        {
            int _cols = 7;
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