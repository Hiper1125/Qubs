namespace Towy.Utilities
{
    /// <summary>
    /// Interface for data store
    /// </summary>
    public interface IDataStore
    {
        void Init();
        void PreSave();
        void PostLoad();
    }
}
