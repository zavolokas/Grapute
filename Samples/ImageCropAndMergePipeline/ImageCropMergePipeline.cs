using System;
using System.Drawing;
using ChainPipelinesSample.Tasks;
using Grapute;

namespace ChainPipelinesSample
{
    internal class ImageCropMergePipeline : Node<Bitmap, Bitmap>
    {
        private readonly Size _size1;
        private readonly Size _size2;
        private readonly int _cols;

        public ImageCropMergePipeline(Size size1, Size size2, int cols)
        {
            _size1 = size1;
            _size2 = size2;
            _cols = cols;
        }

        protected override Bitmap[] Process(Bitmap bitmap)
        {
            return new MarkBitmapParts(_size1.Width, _size1.Height)
                .SetInput(bitmap)
                .ForEachOutput(new ExtractMarkedBitmapPart())
                .ForEachOutput(new MarkBitmapParts(_size2.Width, _size2.Height))
                .ForEachOutput(new ExtractMarkedBitmapPart())
                .CollectAllOutputsToOneArray()
                .ForArray(new MergeBitmapsToOne(_cols))
                .Process()
                .Output;
        }
    }
}