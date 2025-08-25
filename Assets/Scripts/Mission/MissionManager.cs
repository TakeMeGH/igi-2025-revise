using Perspective.Character.NPC;
using Perspective.UI;
using Perspective.Utils;
using UnityEngine;

namespace Perspective.Mission
{
    public class MissionManager : MonoBehaviour
    {
        [Header("Missions (index = day-1)")]
        [Tooltip("Assign up to 5 mission data assets (Day 1 = index 0, Day 2 = index 1, etc.)")]
        [SerializeField]
        private MissionData[] dayMissions = new MissionData[5];

        private MissionData currentMission;
        public MissionData CurrentMission => currentMission;
        [SerializeField] private MissionController missionController;

        private void Start()
        {
            int currentDay = DataManager.Instance != null ? DataManager.Instance.currentDay : 1;
            SetDayMission(currentDay);
            missionController.ShowMission(currentMission);
        }

        private void SetDayMission(int day)
        {
            if (day < 1 || day > 5)
            {
                Debug.LogWarning("Day must be between 1 and 5!");
                return;
            }

            currentMission = dayMissions[day - 1];
            if (currentMission == null)
            {
                Debug.LogWarning($"No mission assigned for Day {day}");
                return;
            }

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

            // Refresh the UI
            if (missionController != null)
                missionController.RefreshMissionUI();

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