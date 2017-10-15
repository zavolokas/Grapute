namespace Grapute.Jobs
{
    /// <summary>
    /// Defines methods for manipulations with the data storage.
    /// </summary>
    public interface IJobDataStorage
    {
        /// <summary>
        /// Generates an unique data identifyer.
        /// </summary>
        /// <returns></returns>
        DataIdentifyer GenerateDataIdentifyer(object data);

        /// <summary>
        /// Obtains a data identifyed by the id from the storage.
        /// </summary>
        /// <typeparam name="T">The type of the obtained data.</typeparam>
        /// <param name="id">The data unique id.</param>
        /// <returns></returns>
        T GetData<T>(DataIdentifyer id) where T: class;

        /// <summary>
        /// Saves the specifyed data in the storage.
        /// </summary>
        /// <typeparam name="T">The type of the saved data.</typeparam>
        /// <param name="data">The data to save.</param>
        /// <param name="id">The data unique identifyer in the storage.</param>
        /// <returns></returns>
        bool SaveData<T>(T data, DataIdentifyer id);
    }
}