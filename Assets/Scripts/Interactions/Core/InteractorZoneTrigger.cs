using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Perspective.Interactions.Core
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool, IInteractable> { }
    public class InteractorZoneTrigger : MonoBehaviour
    {
        [FormerlySerializedAs("_enterZone")] [SerializeField] private BoolEvent enterZone;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable currentInteraction))
            {
                enterZone.Invoke(true, currentInteraction);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractable currentInteraction))
            {
                enterZone.Invoke(false, currentInteraction);
            }
        }
    }
}
