using System;
using System.IO;
using PipelinesLib.Jobs;
using Zavolokas.ParallelComputing.Jobs;
using Zavolokas.ParallelComputing.Jobs.Serialization;

namespace PipelinesLib
{
    public class ImgProcJobBinaryDeserializer : JobBinaryDeserializer
    {
        //todo: violates single choice principle - duplication in the serializer
        private const byte ExtractJobTypeId = 1;
        private const byte MergeJobTypeId = 2;
        private const byte MarkupJobTypeId = 3;

        public ImgProcJobBinaryDeserializer(IJobDataStorage jobDataStorage)
            : base(jobDataStorage)
        {
        }

        protected override IJob ReadJob(BinaryReader reader)
        {
            var jobType = reader.ReadByte();
            if (jobType == ExtractJobTypeId)
            {
                return ReadExtractJob(reader);
            }
            if (jobType == MergeJobTypeId)
            {
                return ReadMergeJob(reader);
            }

            if (jobType == MarkupJobTypeId)
            {
                return ReadMarkupJob(reader);
            }

            //todo: throw more speific exception
            throw new Exception("Attemt to read unknown type of job");
        }

        private MarkupBitmapJob ReadMarkupJob(BinaryReader reader)
        {
            var job = new MarkupBitmapJob(JobDataStorage, 0);
            ReadBaseJob(reader, job);
            job.Width = reader.ReadInt32();
            job.Height = reader.ReadInt32();
            return job;
        }

        private IJob ReadMergeJob(BinaryReader reader)
        {
            var job = new MergeBitmapsJob(JobDataStorage, 0);
            ReadBaseJob(reader, job);
            job.Cols = reader.ReadInt32();
            return job;
        }

        private IJob ReadExtractJob(BinaryReader reader)
        {
            // todo: passing the priority is not necessery
            var job = new ExtractMarkedBitmapPartJob(JobDataStorage, 0);
            ReadBaseJob(reader, job);
            return job;
        }
    }
}