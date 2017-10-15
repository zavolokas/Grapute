using System.Collections.Generic;

namespace Grapute.Jobs
{
    /// <summary>
    /// Base class for a task that creates jobs to process input data into the output data.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <seealso cref="Task{TInput,TOutput}" />
    public abstract class JobProducerTaskBase<TInput, TOutput> : Task<TInput, TOutput>
    {
        private readonly List<IJob> _jobs;
        private readonly IJobDataStorage _jobDataStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobProducerTask{TInput,TOutput}"/> class.
        /// </summary>
        /// <param name="jobs">The collection to store created jobs.</param>
        /// <param name="jobDataStorage">The data storage where all the needed data will be stored.</param>
        protected JobProducerTaskBase(List<IJob> jobs, IJobDataStorage jobDataStorage)
        {
            _jobs = jobs;
            _jobDataStorage = jobDataStorage;
        }

        /// <summary>
        /// Gets the job data storage.
        /// </summary>
        /// <value>
        /// The job data storage.
        /// </value>
        protected IJobDataStorage JobDataStorage
        {
            get { return _jobDataStorage; }
        }

        /// <summary>
        /// Adds the job.
        /// </summary>
        /// <param name="job">The job.</param>
        protected void AddJob(IJob job)
        {
            _jobs.Add(job);
        }
    }
}