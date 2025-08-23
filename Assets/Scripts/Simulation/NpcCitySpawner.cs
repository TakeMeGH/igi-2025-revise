using System.Collections;
using System.Collections.Generic;
using Perspective.Character.NPC;
using UnityEngine;

namespace Perspective.Simulation
{
    [System.Serializable]
    public class NpcSpawnLimit
    {
        public NpcType type;
        public int maxCount;
        [HideInInspector] public int currentCount;
    }

    public class NpcCitySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")] [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnInterval = 5f; // seconds between spawn attempts
        [SerializeField] private int initialTrySpawn = 6;
        [Header("NPC Prefabs")] [SerializeField] private GameObject civilianPrefab;
        [SerializeField] private GameObject thiefPrefab;
        [SerializeField] private GameObject brawlerPrefab;

        [Header("Type Limits")] [SerializeField] private List<NpcSpawnLimit> spawnLimits = new();

        private Dictionary<NpcType, GameObject> npcPrefabs;

        private void Awake()
        {
            npcPrefabs = new Dictionary<NpcType, GameObject>
            {
                { NpcType.Civilian, civilianPrefab },
                { NpcType.Thief, thiefPrefab },
                { NpcType.Brawler, brawlerPrefab }
            };
        }

        private void Start()
        {
            for (int i = 0; i < initialTrySpawn; i++)
            {
                TrySpawnRandomNpc();
            }
            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                TrySpawnRandomNpc();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void TrySpawnRandomNpc()
        {
            if (spawnPoints.Length == 0) return;

            NpcType type = (NpcType)Random.Range(0, System.Enum.GetValues(typeof(NpcType)).Length);
            var limit = spawnLimits.Find(x => x.type == type);

            if (limit == null || limit.currentCount >= limit.maxCount)
                return;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject npc = Instantiate(npcPrefabs[type], spawnPoint.position, spawnPoint.rotation);

            limit.currentCount++;

            NpcController controller = npc.GetComponent<NpcController>();
            controller.OnNpcDestroyed += () => { limit.currentCount--; };
        }
    }
}