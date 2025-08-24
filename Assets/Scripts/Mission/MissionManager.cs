using Perspective.Character.NPC;
using UnityEngine;

namespace Perspective.Mission
{
    public class MissionManager : MonoBehaviour
    {
        [SerializeField] private MissionData currentMission;

        public MissionData CurrentMission => currentMission;

        private void Start()
        {
            if (currentMission != null)
                ResetMission();
        }

        public void ResetMission()
        {
            if (currentMission == null) return;

            foreach (var obj in currentMission.objectives)
                obj.currentCount = 0;
        }

        public void OnPhotoTaken(NpcEvent npcEvent)
        {
            if (currentMission == null) return;

            currentMission.RegisterNpcEventPhoto(npcEvent);

            Debug.Log($"Photo registered: {npcEvent}. Mission progress: {GetMissionProgress()}");

            if (currentMission.IsMissionComplete)
            {
                Debug.Log("Mission complete!");
                // TODO: Trigger mission completion rewards/cutscene/etc.
            }
        }

        public string GetMissionProgress()
        {
            if (currentMission == null) return "No mission loaded.";

            string progress = "";
            foreach (var obj in currentMission.objectives)
            {
                progress += $"{obj.description}: {obj.currentCount}/{obj.requiredCount}\n";
            }

            return progress;
        }
    }
}