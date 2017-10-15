// The purpose of this sample is to demonstrate how to create a processing pipelines. 
// It doesn't show a real case scenario, moreover it is overcomplicated for the sake of the sample code.
//
// In this sample a processing pipeline is created that repeats the following sequense of actions: 
//  - creates a markups for each of the input bitmaps
//  - based on the markup, extracts more bitmaps
// After that it merges all the bitmaps to a single one.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Grapute;
using Zavolokas.Utils.Processes;

namespace ImageShufflePipeline
{
    class Program
    {
        static void Main(string[] args)
        {
            var functions = new ImageProcessingFunctions();

            // Keep the start node separately to provide an input
            var startNode = new FuncNode<Bitmap, BitmapRegion>(functions.DivideIn4Regions);

            var pipeline = startNode
                .ForEachOutput(x => new[] { functions.ExtractToNewBitmap(x) })
                .ForEachOutput(x => functions.DivideIn4Regions(x))
                .ForEachOutput(x => new[] { functions.ExtractToNewBitmap(x) })
                .ForEachOutput(x => functions.DivideIn4Regions(x))
                .ForEachOutput(x => new[] { functions.ExtractToNewBitmap(x) })
                .CollectAllOutputsToOneArray()
                .ForArray(x => new[] { functions.MergeRegions(x) })
                .ForEachOutput(x =>
                {
                    var fi = new FileInfo(@"..\..\output.png");
                    x.Save(fi.FullName, ImageFormat.Png);
                    return new[] { fi };
                });

            var filePath = "..\\..\\..\\images\\image1.jpg";
            using (var bitmap = new Bitmap(filePath))
            {
                startNode.SetInput(bitmap);

                pipeline
                    .Process()
                    .Output[0]
                    .ShowFile();
            }

            Console.ReadLine();

            filePath = "..\\..\\..\\images\\image2.jpg";
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
