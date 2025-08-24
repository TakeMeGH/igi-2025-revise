using System.Collections.Generic;
using Perspective.Mission;
using TMPro;
using UnityEngine;

namespace Perspective.UI
{
    public class MissionController : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Parent object where mission UI entries will be spawned (e.g., a Vertical Layout Group).")]
        [SerializeField]
        private Transform missionListParent;

        [Tooltip("Prefab for a single mission objective UI element.")]
        [SerializeField]
        private GameObject missionEntryPrefab;

        private readonly List<GameObject> spawnedEntries = new List<GameObject>();

        /// <summary>
        /// Spawns mission objectives UI under the parent object.
        /// </summary>
        public void ShowMission(MissionData mission)
        {
            if (mission == null) return;

            ClearMissionUI();

            foreach (var obj in mission.objectives)
            {
                GameObject entry = Instantiate(missionEntryPrefab, missionListParent);
                entry.name = $"MissionEntry_{obj.targetEvent}";

                // If prefab has a script for displaying text, update it
                var ui = entry.GetComponent<TMP_Text>();
                if (ui != null)
                {
                    ui.SetText(obj.description);
                }

                spawnedEntries.Add(entry);
            }
        }

        /// <summary>
        /// Clears all spawned UI entries.
        /// </summary>
        private void ClearMissionUI()
        {
            foreach (var go in spawnedEntries)
            {
                if (go != null)
                    Destroy(go);
            }

            spawnedEntries.Clear();
        }
    }
}