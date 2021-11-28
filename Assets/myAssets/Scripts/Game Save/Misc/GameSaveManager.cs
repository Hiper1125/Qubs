using System;
using UnityEngine;

namespace Towy.Utilities
{
    public class GameSaveManager : Singleton<GameSaveManager>
    {
        /// <summary>
        /// Set up persistence
        /// </summary>
        public static T LoadData<T>(string savedGameFile) where T : IDataStore, new()
        {
            JsonSaver<T> dataSaver;
            T dataStore;

#if UNITY_EDITOR
            dataSaver = new JsonSaver<T>(savedGameFile);
#else
			dataSaver = new EncryptedJsonSaver<T>(savedGameFile);
#endif

            try
            {
                if (!dataSaver.Load(out dataStore))
                {
                    dataStore = new T();
                    dataStore.Init();
                    SaveData(savedGameFile, dataStore);
                }
            }
            catch (Exception)
            {
                Debug.Log("Failed to load data, resetting");
                dataStore = new T();
                dataStore.Init();
                SaveData(savedGameFile, dataStore);
            }

            return dataStore;
        }

        /// <summary>
        /// Saves the gamme
        /// </summary>
        public static void SaveData<T>(string savedGameFile, T dataStore) where T : IDataStore
        {
            JsonSaver<T> dataSaver;

#if UNITY_EDITOR
            dataSaver = new JsonSaver<T>(savedGameFile);
#else
			dataSaver = new EncryptedJsonSaver<T>(savedGameFile);
#endif

            dataSaver.Save(dataStore);
        }
    }
}
