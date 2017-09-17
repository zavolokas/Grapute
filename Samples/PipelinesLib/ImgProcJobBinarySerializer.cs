using System;
using System.IO;
using System.Xml.Xsl;
using PipelinesLib.Jobs;
using Zavolokas.ParallelComputing.Jobs;
using Zavolokas.ParallelComputing.Jobs.Serialization;

namespace PipelinesLib
{
    public class ImgProcJobBinarySerializer : JobBinarySerializer, IJobSerializer
    {
        private const byte ExtractJobTypeId = 1;
        private const byte MergeJobTypeId = 2;
        private const byte MarkupJobTypeId = 3;

        protected override void WriteJob(IJob job, BinaryWriter writer)
        {
            var extractJob = job as ExtractMarkedBitmapPartJob;
            if (extractJob != null)
            {
                WriteJob(extractJob, writer);
                return;
            }

            var mergeJob = job as MergeBitmapsJob;
            if (mergeJob != null)
            {
                WriteJob(mergeJob, writer);
                return;
            }

            var markupJob = job as MarkupBitmapJob;
            if (markupJob != null)
            {
                WriteMarkupJob(markupJob, writer);
                return;
            }


            //todo: throw more specific exception when the job type is unknown.
            throw new Exception($"Job can not be serialized because the job type '{job.GetType()}' is unknown.");
        }

        private void WriteMarkupJob(MarkupBitmapJob job, BinaryWriter w)
        {
            w.Write(MarkupJobTypeId);
            WriteBase(job, w);
            w.Write(job.Width);
            w.Write(job.Height);
        }

        private void WriteJob(ExtractMarkedBitmapPartJob job, BinaryWriter w)
        {
            w.Write(ExtractJobTypeId);
            WriteBase(job, w);
        }

        private void WriteJob(MergeBitmapsJob job, BinaryWriter w)
        {
            w.Write(MergeJobTypeId);
            WriteBase(job, w);
            w.Write(job.Cols);
        }
    }
}