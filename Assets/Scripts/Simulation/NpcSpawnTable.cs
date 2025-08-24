using System.Collections.Generic;
using UnityEngine;

namespace Perspective.Simulation
{
    [CreateAssetMenu(fileName = "NpcSpawnTable", menuName = "Perspective/NPC Spawn Table", order = 0)]
    public class NpcSpawnTable : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public List<GameObject> prefabs = new();
            public int maxCount = 10;
            [Range(0f, 1f)] public float spawnWeight = 1f;

            [HideInInspector] public int currentCount; // runtime
            [HideInInspector] public int nextPrefabIndex; // runtime, sequential spawn
        }

        public List<Entry> entries = new();
    }
}