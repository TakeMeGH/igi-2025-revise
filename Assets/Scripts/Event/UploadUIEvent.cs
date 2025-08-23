using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perspective.Event
{
    [CreateAssetMenu(fileName = "New UploadUIEvent Event", menuName = "Scriptable Objects/Events/UploadUIEvent Event")]
    public class UploadUIEvent : ScriptableObject
    {
        public UnityAction<bool, List<SnapshotData>> EventAction;

        public void RaiseEvent(bool isShowingUI, List<SnapshotData> cameraHistory)
        {
            EventAction?.Invoke(isShowingUI, cameraHistory);
        }
    }
}
