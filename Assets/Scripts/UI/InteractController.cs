using System;
using Perspective.Event;
using TMPro;
using UnityEngine;

namespace Perspective.UI
{
    public class InteractController : MonoBehaviour
    {
        [SerializeField] private InteractionEvent interactionEvent;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text descriptions;

        private void OnEnable()
        {
            interactionEvent.EventAction += SetInteractUI;
        }

        private void OnDisable()
        {
            interactionEvent.EventAction -= SetInteractUI;
        }

        private void SetInteractUI(bool status, string value)
        {
            if (status)
            {
                canvasGroup.alpha = 1;
                
                descriptions.SetText(value);
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }
    }
}