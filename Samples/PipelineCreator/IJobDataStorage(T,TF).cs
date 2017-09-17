namespace PipelineCreator
{
    public interface IJobDataStorage<T, TF>
    {
        T GetData(DataIdentifyer inputId);
        DataIdentifyer SaveData(TF output);
    }
}