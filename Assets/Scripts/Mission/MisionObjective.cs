using Perspective.Character.NPC;
using UnityEngine;

namespace Perspective.Mission
{
    [System.Serializable]
    public class MissionObjective
    {
        [Tooltip("The NPC event the player must capture in photos.")] public NpcEvent targetEvent;

        [Tooltip("How many photos must be taken of this event.")] public int requiredCount = 1;

        [Tooltip("Description of this objective (e.g. 'Take 3 photos of civilians fighting').")]
        [TextArea]
        public string description;

        [HideInInspector] public int currentCount = 0;

        public bool IsComplete => currentCount >= requiredCount;

        public void RegisterPhoto()
        {
            currentCount = Mathf.Min(currentCount + 1, requiredCount);
        }
    }
}