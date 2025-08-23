using UnityEngine;

namespace Perspective.Utils
{
    public sealed class SingletonManager : MonoBehaviour
    {
        private static SingletonManager _instance;

        public static SingletonManager Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindAnyObjectByType<SingletonManager>();

                if (_instance) return _instance;
                
                var singleton = new GameObject(typeof(SingletonManager).Name);
                _instance = singleton.AddComponent<SingletonManager>();
                DontDestroyOnLoad(singleton);
                return _instance;
            }
        }

        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
