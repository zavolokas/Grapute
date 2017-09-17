using System.Drawing;
using System.IO;
using ChainPipelinesSample.Tasks;
using Zavolokas.Utils.Processes;

namespace ChainPipelinesSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "..\\..\\..\\storage\\image.jpg";
            var bitmap = new Bitmap(filePath);

            var pipeline = new ImageCropMergePipeline(new Size(640, 512), new Size(320, 256), 4);
            var pipeline2 = new ImageCropMergePipeline(new Size(320, 256), new Size(160, 128), 8);

            string outputFilePath = pipeline.SetInput(bitmap)
                .ForEachOutput(pipeline2)
                .ForEachOutput(new SaveImageTask())
                .Process()
                .Output[0];

            new FileInfo(outputFilePath).ShowFile();
        }
    }
}
