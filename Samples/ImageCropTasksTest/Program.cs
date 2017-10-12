using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Grapute;
using Zavolokas.Utils.Processes;

namespace ImageCropTasksTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var functions = new ImageProcessingFunctions();

            // Keep the start node separately to provide an input
            var startNode = new Node<Bitmap, BitmapRegion>(functions.DivideIn4Regions);

            // Create the processing pipeline that creates a markups for each 
            // of the input bitmaps, based on this markup 
            var pipeline = startNode
                .ForEachOutput(functions.ExtractToNewBitmap)
                .ForEachOutput(functions.DivideIn4Regions)
                .ForEachOutput(functions.ExtractToNewBitmap)
                .ForEachOutput(functions.DivideIn4Regions)
                .ForEachOutput(functions.ExtractToNewBitmap)
                .CollectAllOutputsToOneArray()
                .ForArray(functions.MergeRegions)
                .ForEachOutput(x =>
                {
                    FileInfo fi = new FileInfo(@"..\..\output.png");
                    x.Save(fi.FullName, ImageFormat.Png);
                    return new[] { fi };
                });

            var filePath = "..\\..\\..\\storage\\image1.jpg";
            using (var bitmap = new Bitmap(filePath))
            {
                startNode.SetInput(bitmap);

                pipeline
                    .Process()
                    .Output[0]
                    .ShowFile();
            }

            Console.ReadLine();

            filePath = "..\\..\\..\\storage\\image2.jpg";
            using (var bitmap = new Bitmap(filePath))
            {
                startNode.SetInput(bitmap);

                pipeline
                    .Process()
                    .Output[0]
                    .ShowFile();
            }
        }
    }
}
