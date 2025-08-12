using System;
using UnityEngine;
using UnityEngine.Events;

namespace Perspective
{
    [CreateAssetMenu(fileName = "New Interaction Event", menuName = "Scriptable Objects/Events/Interaction Event")]
    public class InteractionEvent : ScriptableObject
    {
        public UnityAction<bool, String> EventAction;

        public void RaiseEvent(bool isShowingUI, String instruction)
        {
            EventAction?.Invoke(isShowingUI, instruction);
        }
    }
}
