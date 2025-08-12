using UnityEngine;

namespace Perspective
{
    public class Bed : MonoBehaviour, IInteractable
    {
        [SerializeField] string _interactionMessage;
        [SerializeField] bool _isInteractable;
        public string InteractionPrompt => _interactionMessage;

        public bool IsInteractable => _isInteractable;

        public void Interact()
        {
            Debug.Log("Interact Bed");
        }
    }
}
