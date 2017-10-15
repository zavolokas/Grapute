using System;
using System.Collections.Generic;
using System.Linq;

namespace Grapute.Jobs
{
    public abstract class MergeJobProducerTask<TInput, TOutput> : JobProducerTaskBase<JobData<TInput>[], JobData<TOutput>>
        where TInput : class
    {
        protected MergeJobProducerTask(List<IJob> jobs, IJobDataStorage jobDataStorage) 
            : base(jobs, jobDataStorage)
        {
        }

        protected override JobData<TOutput>[] Process(JobData<TInput>[] inputs)
        {
            var job = CreateJob();
            if (job == null)
                //todo: throw more specific exception
                throw new Exception("Created job can not be null");

            var ids = inputs.Select(b => b.Id).ToArray();
            job.Input = JobDataStorage.GenerateDataIdentifyer(ids);
            JobDataStorage.SaveData(ids, job.Input);

            var results = Process(inputs.Select(i=>i.Data).ToArray());

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

        protected abstract TOutput[] Process(TInput[] input);
        protected abstract Job<TInput[], TOutput> CreateJob();
    }
}