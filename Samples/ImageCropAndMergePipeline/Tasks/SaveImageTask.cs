using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Grapute;

namespace ChainPipelinesSample.Tasks
{
    public class SaveImageTask: Task<Bitmap, string>
    {
        protected override string[] Process(Bitmap input)
        {
            string fileName = $"F{Guid.NewGuid()}.png";
            string filePath = Path.Combine("..\\..\\..\\storage\\", fileName);
            input.Save(filePath, ImageFormat.Png);
            return new[] { filePath };
        }
    }
}