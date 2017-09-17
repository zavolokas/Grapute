using System;
using System.Drawing;
using Grapute;

namespace ChainPipelinesSample.Tasks
{
    public class ExtractMarkedBitmapPart : Task<BitmapWithMarkup, Bitmap>
    {
        protected override Bitmap[] Process(BitmapWithMarkup input)
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
    }
}