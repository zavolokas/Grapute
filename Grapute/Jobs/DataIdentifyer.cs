namespace Grapute.Jobs
{
    /// <summary>
    /// The unique job data identifyer.
    /// </summary>
    public class DataIdentifyer
    {
        /// <summary>
        /// The id of a data.
        /// </summary>
        public string Id;

        public override string ToString()
        {
            return $"DataIdentifyer [{Id}]";
        }
    }
}