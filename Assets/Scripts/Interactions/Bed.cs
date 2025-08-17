using Perspective.Interactions.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Perspective.Interactions
{
    public class Bed : MonoBehaviour, IInteractable
    {
        [FormerlySerializedAs("_interactionMessage")] [SerializeField] private string interactionMessage;
        [FormerlySerializedAs("_isInteractable")] [SerializeField] private bool isInteractable;
        public string InteractionPrompt => interactionMessage;

        public bool IsInteractable => isInteractable;

        public void Interact()
        {
            Debug.Log("Interact Bed");
        }
    }
}
