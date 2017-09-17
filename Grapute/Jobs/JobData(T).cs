namespace Grapute.Jobs
{
    /// <summary>
    /// Sores a data (input/output) that is required by a job.
    /// </summary>
    /// <typeparam name="T">Type of the stored data.</typeparam>
    public class JobData<T>
    {
        /// <summary>
        /// Gets or sets the data unique identifier.
        /// </summary>
        /// <value>
        /// The unique data identifier.
        /// </value>
        public DataIdentifyer Id { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public T Data { get; set; }
    }
}