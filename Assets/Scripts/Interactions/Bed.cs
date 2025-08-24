using Perspective.Interactions.Core;
using Perspective.Utils;
using UnityEngine;

namespace Perspective.Interactions
{
    public class Bed : MonoBehaviour, IInteractable
    {
        public string interactionMessage;
        public bool isInteractable;
        public string InteractionPrompt => interactionMessage;

        public bool IsInteractable => isInteractable;

        public void Interact()
        {
            FindAnyObjectByType<LevelManager>().TransitionNextDay();
        }
    }
}