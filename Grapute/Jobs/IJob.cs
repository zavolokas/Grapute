namespace Grapute.Jobs
{
    /// <summary>
    /// Defines a job's properties.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Gets or sets the identifyer that is used to get the input data for the job.
        /// </summary>
        /// <value>
        /// The input data identifyer.
        /// </value>
        DataIdentifyer Input { get; set; }

        /// <summary>
        /// Gets or sets the value of needed compute resources to run the job.
        /// </summary>
        /// <value>
        /// The needed compute resources.
        /// </value>
        /// <remarks>Helpfull to decide wheather the job is a lightweight and can be combined with other jobs.</remarks>
        int RequiredComputeResources { get; set; }

        /// <summary>
        /// Gets or sets the identifyer that is used to save the output data of the job.
        /// </summary>
        /// <value>
        /// The output data identifyer.
        /// </value>
        DataIdentifyer[] Outputs { get; set; }

        /// <summary>
        /// Gets or sets the priority of the job.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        int Priority { get; set; }

        /// <summary>
        /// Runs the job.
        /// </summary>
        void Process();

        /// <summary>
        /// Initializes job by setting a data storage.
        /// </summary>
        /// <param name="storage">The data storage.</param>
        void Init(IJobDataStorage storage);
    }
}