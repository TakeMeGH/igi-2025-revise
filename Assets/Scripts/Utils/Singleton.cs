using UnityEngine;

namespace Perspective.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindAnyObjectByType<T>();

                if (_instance) return _instance;
                var singleton = new GameObject(typeof(T).Name);
                _instance = singleton.AddComponent<T>();
                return _instance;
            }
        }

        private void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                OwnAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OwnAwake()
        {
            
        }

    }}
