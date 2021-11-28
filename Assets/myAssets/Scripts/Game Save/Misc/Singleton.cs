using UnityEngine;

namespace Towy.Utilities
{
    /// <summary>
    /// Singleton class
    /// </summary>
    /// <typeparam name="T">Type of the singleton</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        /// <summary>
        /// The static reference to the instance
        /// </summary>
        public static T Instance { get; protected set; }

        /// <summary>
        /// Gets whether an instance of this singleton exists
        /// </summary>
        public static bool InstanceExists
        {
            get { return Instance != null; }
        }

        /// <summary>
        /// Awake method to associate singleton with instance
        /// </summary>
        protected virtual void Awake()
        {
            if (InstanceExists)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = (T)this;
                gameObject.layer = 31;
            }

            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        /// <summary>
        /// OnDestroy method to clear singleton association
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}