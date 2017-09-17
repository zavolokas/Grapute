using System;
using System.Collections.Generic;

namespace Grapute.Jobs
{
    public abstract class JobProducerTask<TInput, TOutput> : JobProducerTaskBase<JobData<TInput>, JobData<TOutput>>
        where TInput : class
    {
        protected JobProducerTask(List<IJob> jobs, IJobDataStorage jobDataStorage)
            : base(jobs, jobDataStorage)
        {
        }

        protected override JobData<TOutput>[] Process(JobData<TInput> input)
        {
            var job = CreateJob();
            if (job == null)
                //todo: throw more specific exception
                throw new Exception("Created job can not be null");

            job.Input = input.Id;

            var results = Process(input.Data);

            job.Outputs = new DataIdentifyer[results.Length];
            var outputs = new JobData<TOutput>[results.Length];

            for (var i = 0; i < outputs.Length; i++)
            {
                var output = new JobData<TOutput>
                {
                    Data = results[i],
                    Id = JobDataStorage.GenerateDataIdentifyer(results[i])
                };

                outputs[i] = output;
                job.Outputs[i] = output.Id;
            }

            AddJob(job);
            return outputs;
        }

        public abstract TOutput[] Process(TInput input);
        protected abstract Job<TInput, TOutput> CreateJob();
    }
}