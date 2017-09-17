using System.Collections.Generic;
using System.Drawing;
using Zavolokas.ParallelComputing.Tasks;

namespace ImageCropTasksTest
{
    public class MarkupBitmapTask : Task<Bitmap, BitmapWithMarkup>
    {
        private readonly int _width;
        private readonly int _height;

        public MarkupBitmapTask(int width, int height)
        {
            _width = width;
            _height = height;
        }

        protected override BitmapWithMarkup[] Process(Bitmap input)
        {
            var result = new List<BitmapWithMarkup>();

            int wp = input.Width/_width;
            int hp = input.Height/_height;

            for (int y = 0; y < hp; y++)
            {
                for (int x = 0; x < wp; x++)
                {
                    var arg = new BitmapWithMarkup() {Bmp = input, X = x*_width, Y = y*_height, Width = _width, Height = _height};
                    result.Add(arg);
                }
            }

            return result.ToArray();
        }
    }
}