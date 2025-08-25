using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Perspective.Character.NPC;
using Perspective.Utils;
using UnityEngine;

namespace Perspective.Simulation
{
    public class NpcCitySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")] [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private float spawnIntervalJitter = 2f;
        [SerializeField] private float spawnPointCooldown = 8f;
        [SerializeField] private int initialTrySpawn = 6;

        [Header("Spawn Tables (index = day-1)")]
        [Tooltip("Assign up to 5 spawn tables (Day 1 = index 0, Day 2 = index 1, etc.)")]
        [SerializeField]
        private NpcSpawnTable[] daySpawnTables = new NpcSpawnTable[15];

        private NpcSpawnTable activeSpawnTable;
        private Dictionary<Transform, float> spawnPointCooldowns;
        private Coroutine spawnCoroutine;

        private void Start()
        {
            int currentDay = DataManager.Instance != null ? DataManager.Instance.currentDay : 1;
            SetDay(currentDay); // initialize with DataManager
            InitCooldowns();
        }

        private void InitCooldowns()
        {
            spawnPointCooldowns = new Dictionary<Transform, float>();
            foreach (var sp in spawnPoints)
                spawnPointCooldowns[sp] = 0f;
        }

        /// <summary>
        /// Change the day and load the corresponding spawn table.
        /// </summary>
        private void SetDay(int day)
        {
            if (day < 1 || day > 5)
            {
                Debug.LogWarning("Day must be between 1 and 5!");
                return;
            }

            activeSpawnTable = daySpawnTables[day - 1];

            if (!activeSpawnTable)
            {
                Debug.LogWarning($"No spawn table assigned for Day {day}.");
                return;
            }

            // reset spawn entry counters
            foreach (var e in activeSpawnTable.entries)
            {
                e.currentCount = 0;
                e.nextPrefabIndex = 0;
            }
        }

        /// <summary>
        /// Starts the NPC spawner loop (can be called manually from other scripts).
        /// </summary>
        public void StartSpawner()
        {
            if (spawnCoroutine != null) return; // prevent multiple loops

            for (int i = 0; i < initialTrySpawn; i++)
                TrySpawnRandomNpc();

            spawnCoroutine = StartCoroutine(SpawnLoop());
        }

        /// <summary>
        /// Stops the NPC spawner loop.
        /// </summary>
        public void StopSpawner()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                TrySpawnRandomNpc();
                var next = spawnInterval + Random.Range(-spawnIntervalJitter, spawnIntervalJitter);
                yield return new WaitForSeconds(Mathf.Max(0.1f, next));
            }
        }

        private void TrySpawnRandomNpc()
        {
            if (activeSpawnTable == null) return;

            List<Transform> availablePoints = new List<Transform>();
            foreach (var sp in spawnPoints)
            {
                if (Time.time >= spawnPointCooldowns[sp])
                    availablePoints.Add(sp);
            }

            if (availablePoints.Count == 0) return;

            var entry = PickWeightedEntry();
            if (entry == null || entry.prefabs.Count == 0) return;

            var spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
            var offset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

            // Sequential prefab selection
            var prefabToSpawn = entry.prefabs[entry.nextPrefabIndex];
            entry.nextPrefabIndex = (entry.nextPrefabIndex + 1) % entry.prefabs.Count;

            GameObject npc = Instantiate(
                prefabToSpawn,
                spawnPoint.position + offset,
                Quaternion.Euler(0, Random.Range(0f, 360f), 0)
            );

            spawnPointCooldowns[spawnPoint] = Time.time + spawnPointCooldown;
            entry.currentCount++;

            npc.transform.localScale *= Random.Range(0.9f, 1.1f);

            var controller = npc.GetComponent<NpcController>();
            if (controller != null)
                controller.OnNpcDestroyed += () => { entry.currentCount--; };
        }

        private NpcSpawnTable.Entry PickWeightedEntry()
        {
            var available = activeSpawnTable.entries.FindAll(e => e.currentCount < e.maxCount);
            if (available.Count == 0) return null;

            var totalWeight = available.Sum(e => e.spawnWeight);
            var pick = Random.value * totalWeight;
            var cumulative = 0f;

            foreach (var e in available)
            {
                cumulative += e.spawnWeight;
                if (pick <= cumulative)
                    return e;
            }

            return available[0];
        }
    }
}