using UnityEngine;

namespace Perspective.Utils
{
    public class ColdSingletonLoader : MonoBehaviour
    {
        [SerializeField] private GameObject managers;
        private void Awake()
        {
            if (!FindAnyObjectByType<SingletonManager>())
            {
                Instantiate(managers);
            }
        }
    }
}