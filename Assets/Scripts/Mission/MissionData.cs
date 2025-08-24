using Perspective.Character.NPC;
using UnityEngine;

namespace Perspective.Mission
{
    [CreateAssetMenu(fileName = "NewMissionData", menuName = "Missions/MissionData")]
    public class MissionData : ScriptableObject
    {
        [Header("Objectives")] public MissionObjective[] objectives;

        public bool IsMissionComplete => objectives != null && objectives.Length > 0 && AllObjectivesComplete();

        private bool AllObjectivesComplete()
        {
            foreach (var obj in objectives)
            {
                if (!obj.IsComplete) return false;
            }

            return true;
        }

        public void RegisterNpcEventPhoto(NpcEvent npcEvent)
        {
            foreach (var obj in objectives)
            {
                if (obj.targetEvent == npcEvent && !obj.IsComplete)
                {
                    obj.RegisterPhoto();
                    break;
                }
            }
        }
    }
}