using System.Collections.Generic;
using Perspective.Mission;
using UnityEngine;

namespace Perspective.UI
{
    public class MissionController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform missionListParent;
        [SerializeField] private GameObject missionEntryPrefab;

        private readonly List<MissionEntryUI> spawnedEntries = new List<MissionEntryUI>();

        /// <summary>
        /// Spawns mission objectives UI under the parent object.
        /// </summary>
        public void ShowMission(MissionData mission)
        {
            if (mission == null) return;

            ClearMissionUI();

            foreach (var obj in mission.objectives)
            {
                GameObject entryGO = Instantiate(missionEntryPrefab, missionListParent);
                entryGO.name = $"MissionEntry_{obj.targetEvent}";

                var entryUI = entryGO.GetComponent<MissionEntryUI>();
                Debug.Log(entryUI + " DEBUG" );
                if (entryUI != null)
                    entryUI.Initialize(obj);

                spawnedEntries.Add(entryUI);
            }
        }

        /// <summary>
        /// Clears all spawned UI entries.
        /// </summary>
        private void ClearMissionUI()
        {
            foreach (var entry in spawnedEntries)
            {
                if (entry != null)
                    Destroy(entry.gameObject);
            }
            spawnedEntries.Clear();
        }

        /// <summary>
        /// Updates all mission UI entries (call after a photo is taken).
        /// </summary>
        public void RefreshMissionUI()
        {
            foreach (var entry in spawnedEntries)
            {
                if (entry != null)
                    entry.UpdateUI();
            }
        }
    }
}