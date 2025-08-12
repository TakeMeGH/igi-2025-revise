using UnityEngine;
using UnityEngine.Events;

namespace Perspective
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool, IInteractable> { }
    public class InteractorZoneTrigger : MonoBehaviour
    {
        [SerializeField] private BoolEvent _enterZone = default;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other);
            if (other.TryGetComponent<IInteractable>(out IInteractable currentInteraction))
            {
                _enterZone.Invoke(true, currentInteraction);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out IInteractable currentInteraction))
            {
                _enterZone.Invoke(false, currentInteraction);
            }
        }
    }
}
