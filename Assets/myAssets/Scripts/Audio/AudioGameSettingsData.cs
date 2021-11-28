using System;

namespace Towy.Utilities
{
    [Serializable]
    public class AudioGameSettingsData : IDataStore
    {
        public bool musicMuted;
        public bool soundMuted;

        public float masterVolume;
        public float musicVolume;
        public float soundVolume;

        public void Init()
        {
            musicMuted = false;
            soundMuted = false;

            masterVolume = 1.0f;
            musicVolume = 1.0f;
            soundVolume = 1.0f;
        }

        /// <summary>
        /// Called just before we save
        /// </summary>
        public void PreSave()
        {
        }

        /// <summary>
        /// Called just after load
        /// </summary>
        public void PostLoad()
        {
        }
    }
}

