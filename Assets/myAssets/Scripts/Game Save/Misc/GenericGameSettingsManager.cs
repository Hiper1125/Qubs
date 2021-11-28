using System;

namespace Towy.Utilities
{
    public class GenericGameSettingsManager<M, D> : Singleton<M> where D : IDataStore, new() where M : GenericGameSettingsManager<M, D>
    {
        /// <summary>
        /// Is ready
        /// </summary>
        private bool m_IsReady = false;

        /// <summary>
        /// The serialization implementation for persistence 
        /// </summary>
        private JsonSaver<D> m_DataSaver;

        /// <summary>
        /// The object used for persistence
        /// </summary>
        private D m_DataStore;

        /// <summary>
        /// Return Game Settings
        /// </summary>
        public static D GameSettingsData
        {
            get
            {
                if (Instance.m_DataStore == null)
                    Instance.LoadData();

                return Instance.m_DataStore;
            }
            private set { }
        }

        protected override void OnAwake()
        {
            LoadData();
        }

        protected void SaveData()
        {
            m_DataSaver.Save(m_DataStore);
        }

        protected void LoadData()
        {
            string savedGameFileName = typeof(D).Name;

#if UNITY_EDITOR
            m_DataSaver = new JsonSaver<D>(savedGameFileName);
#else
            m_DataSaver = new EncryptedJsonSaver<D>(savedGameFileName);
#endif

            try
            {
                if (!m_DataSaver.Load(out m_DataStore))
                {
                    m_DataStore = new D();
                    m_DataStore.Init();
                    SaveData();
                }
            }
            catch (Exception)
            {
                m_DataStore = new D();
                m_DataStore.Init();
                SaveData();
            }

            m_IsReady = true;
        }

        public static bool IsReady()
        {
            return Instance.m_IsReady;
        }

        void OnApplicationQuit()
        {
            SaveData();
        }
    }
}
