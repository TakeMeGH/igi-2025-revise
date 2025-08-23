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
        [Range(0f, 1f)] public float spawnWeight = 1f; // chance weight
    }

    public class NpcCitySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private float spawnIntervalJitter = 2f;
        [SerializeField] private float spawnPointCooldown = 8f; // cooldown for each point
        [SerializeField] private int initialTrySpawn = 6;

        [Header("NPC Prefabs")]
        [SerializeField] private GameObject civilianPrefab;
        [SerializeField] private GameObject thiefPrefab;
        [SerializeField] private GameObject brawlerPrefab;

        [Header("Type Limits")]
        [SerializeField] private List<NpcSpawnLimit> spawnLimits = new();

        private Dictionary<NpcType, GameObject> npcPrefabs;
        private Dictionary<Transform, float> spawnPointCooldowns;

        private void Awake()
        {
            npcPrefabs = new Dictionary<NpcType, GameObject>
            {
                { NpcType.Civilian, civilianPrefab },
                { NpcType.Thief, thiefPrefab },
                { NpcType.Brawler, brawlerPrefab }
            };

            // Initialize cooldown dictionary
            spawnPointCooldowns = new Dictionary<Transform, float>();
            foreach (var sp in spawnPoints)
                spawnPointCooldowns[sp] = 0f;
        }

        private void Start()
        {
            for (int i = 0; i < initialTrySpawn; i++)
                TrySpawnRandomNpc();

            StartCoroutine(SpawnLoop());
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                TrySpawnRandomNpc();
                float next = spawnInterval + Random.Range(-spawnIntervalJitter, spawnIntervalJitter);
                yield return new WaitForSeconds(Mathf.Max(0.1f, next));
            }
        }

        private void TrySpawnRandomNpc()
        {
            // Pick a spawn point that’s off cooldown
            List<Transform> availablePoints = new List<Transform>();
            foreach (var sp in spawnPoints)
            {
                if (Time.time >= spawnPointCooldowns[sp])
                    availablePoints.Add(sp);
            }

            if (availablePoints.Count == 0) return;

            NpcType? type = PickWeightedNpcType();
            if (type == null) return;

            Transform spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            );

            GameObject npc = Instantiate(
                npcPrefabs[type.Value],
                spawnPoint.position + offset,
                Quaternion.Euler(0, Random.Range(0f, 360f), 0)
            );

            // Set cooldown for this spawn point
            spawnPointCooldowns[spawnPoint] = Time.time + spawnPointCooldown;

            var limit = spawnLimits.Find(x => x.type == type.Value);
            limit.currentCount++;

            // Example random variation
            npc.transform.localScale *= Random.Range(0.9f, 1.1f);

            NpcController controller = npc.GetComponent<NpcController>();
            controller.OnNpcDestroyed += () => { limit.currentCount--; };
        }

        private NpcType? PickWeightedNpcType()
        {
            List<NpcSpawnLimit> available = spawnLimits.FindAll(l => l.currentCount < l.maxCount);
            if (available.Count == 0) return null;

            float totalWeight = 0f;
            foreach (var l in available) totalWeight += l.spawnWeight;

            float pick = Random.value * totalWeight;
            float cumulative = 0f;
            foreach (var l in available)
            {
                cumulative += l.spawnWeight;
                if (pick <= cumulative)
                    return l.type;
            }

            return available[0].type; // fallback
        }
    }
}
