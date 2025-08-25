using System.Collections.Generic;
using Perspective.Event;
using Perspective.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Perspective.Interactions.Core
{
    public class Interactor : MonoBehaviour
    {
        [Header("Dependencies")]
        [FormerlySerializedAs("_inputReader")]
        [SerializeField]
        private InputReader inputReader;

        [FormerlySerializedAs("_interactionEvent")] [SerializeField] private InteractionEvent interactionEvent;
        private readonly List<IInteractable> _potentialInteractions = new List<IInteractable>();
        private IInteractable _closestInteractable;

        private void OnEnable()
        {
            if (inputReader)
            {
                inputReader.InteractEvent += OnInteractionButtonPress;
            }
        }

        private void OnDisable()
        {
            if (inputReader)
            {
                inputReader.InteractEvent -= OnInteractionButtonPress;
            }
        }

        private void Update()
        {
            FindClosestInteractable();
            UpdateInteractionEvent();
        }

        public void OnTriggerChangeDetected(bool entered, IInteractable interactable)
        {
            if (entered)
            {
                AddPotentialInteraction(interactable);
            }
            else
            {
                RemovePotentialInteraction(interactable);
            }
        }

        private void FindClosestInteractable()
        {
            _potentialInteractions.RemoveAll(item => item == null || !(item as MonoBehaviour));

            _closestInteractable = null;
            var closestDistance = float.MaxValue;

            foreach (var potential in _potentialInteractions)
            {
                if (!potential.IsInteractable)
                {
                    continue;
                }

                var monoBehaviour = potential as MonoBehaviour;
                if (!monoBehaviour) continue;

                float distance = Vector3.Distance(transform.position, monoBehaviour.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _closestInteractable = potential;
                }
            }
        }

        private void UpdateInteractionEvent()
        {
            if (_closestInteractable == null)
            {
                interactionEvent.RaiseEvent(false, "");
                return;
            }

            interactionEvent.RaiseEvent(true, _closestInteractable.InteractionPrompt);
        }

        private void AddPotentialInteraction(IInteractable interactable)
        {
            if (!_potentialInteractions.Contains(interactable))
            {
                _potentialInteractions.Add(interactable);
            }
        }

        private void RemovePotentialInteraction(IInteractable interactable)
        {
            if (_potentialInteractions.Contains(interactable))
            {
                if (_closestInteractable == interactable)
                {
                    _closestInteractable = null;
                }

                _potentialInteractions.Remove(interactable);
            }
        }

        private void OnInteractionButtonPress()
        {
            if (_closestInteractable is { IsInteractable: true })
            {
                _closestInteractable.Interact();
            }
        }

        public void DisableInteractor()
        {
            if (interactionEvent)
            {
                interactionEvent.RaiseEvent(false, "");
            }

            _potentialInteractions.Clear();
            _closestInteractable = null;

            enabled = false;
        }
        public void EnableInteractor()
        {
            enabled = true;
        }

    }
}