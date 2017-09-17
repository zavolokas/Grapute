using System.Drawing;
using Zavolokas.ParallelComputing.Jobs;

namespace PipelinesLib
{
    /// <summary>
    /// Stores a bitmap and a rectangle within this bitmap.
    /// </summary>
    public class BitmapWithMarkup
    {
        public Bitmap Bmp;
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }
}