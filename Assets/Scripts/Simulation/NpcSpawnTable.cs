using System.Collections.Generic;
using UnityEngine;

namespace Perspective.Simulation
{
    [CreateAssetMenu(fileName = "NpcSpawnTable", menuName = "Scriptable Objects/NPC Spawn Table", order = 0)]
    public class NpcSpawnTable : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public GameObject prefab;
            public int maxCount = 10;
            [Range(0f, 1f)] public float spawnWeight = 1f;
            [HideInInspector] public int currentCount;
        }

        public List<Entry> entries = new();
    }
}