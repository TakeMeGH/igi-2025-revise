using Perspective.Interactions.Core;
using UnityEngine;

namespace Perspective.Interactions
{
    public class Laptop : MonoBehaviour, IInteractable
    {
        public string interactionMessage;
        public bool isInteractable;
        public string InteractionPrompt => interactionMessage;
        public bool IsInteractable => isInteractable;
        private SnapshotCameraController _snapshotCameraController;


        private void Start()
        {
            _snapshotCameraController = FindAnyObjectByType<SnapshotCameraController>();
        }

        public void Interact()
        {
            _snapshotCameraController.SetUploadUI();
        }
    }
}