using System;
using System.Drawing;
using System.IO;
using Zavolokas;
using Zavolokas.Utils.Processes;

namespace ImageCropTasksTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "..\\..\\..\\storage\\image.jpg";

            var bitmap = new Bitmap(filePath);

            string outputFilePath = new MarkupBitmapTask(640, 512)
                .SetInput(bitmap)
                .ForEachOutput(new ExtractBitmapTask())
                .ForEachOutput(new MarkupBitmapTask(160, 128))
                .ForEachOutput(new ExtractBitmapTask())
                .CollectAllOutputsToOneArray()
                .ForArray(new MergeBitmapsToOneTask(3))
                .ForEachOutput(new SaveImageTask())
                .Process()
                .Output[0];

            new FileInfo(outputFilePath).ShowFile();
        }
    }
}
