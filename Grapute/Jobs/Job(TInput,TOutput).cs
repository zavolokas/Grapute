namespace Grapute.Jobs
{
    /// <summary>
    /// Base class for the job that processes the input data into the output data.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TOutput">The type of the output.</typeparam>
    /// <seealso cref="IJob" />
    public abstract class Job<TInput, TOutput> : IJob
        where TInput: class 
    {
        private IJobDataStorage _storage;

        public IJobDataStorage JobDataStorage { get { return _storage;} }

        /// <summary>
        /// Initializes a new instance of the <see cref="Job{TInput, TOutput}" /> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="priority">The priority of the job.</param>
        protected Job(IJobDataStorage storage, int priority)
        {
            _storage = storage;
            Priority = priority;
        }

        /// <summary>
        /// Initializes job by setting a data storage.
        /// </summary>
        /// <param name="storage">The data storage.</param>
        public void Init(IJobDataStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Gets or sets the priority of the job.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the value of required compute resources to run the job.
        /// </summary>
        /// <value>
        /// The required compute resources.
        /// </value>
        /// <remarks>Helpfull to decide wheather the job is a lightweight and can be combined with other jobs.</remarks>
        public int RequiredComputeResources { get; set; }

        /// <summary>
        /// Gets or sets the identifyer that is used to get the input data for the job.
        /// </summary>
        /// <value>
        /// The input data identifyer.
        /// </value>
        public DataIdentifyer Input { get; set; }

        /// <summary>
        /// Gets or sets the identifyers that are used to save the output datas of the job.
        /// </summary>
        /// <value>
        /// The output data identifyers.
        /// </value>
        public DataIdentifyer[] Outputs { get; set; }

        /// <summary>
        /// Runs the job.
        /// </summary>
        public void Process()
        {
            var input = _storage.GetData<TInput>(Input);
            var output = Process(input);
            for (int i = 0; i < output.Length; i++)
            {
                _storage.SaveData<TOutput>(output[i], Outputs[i]);
            }
            
        }

        /// <summary>
        /// Processes the input data into the output data.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <returns></returns>
        protected abstract TOutput[] Process(TInput input);
    }
}