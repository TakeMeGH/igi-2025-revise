using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Perspective.Character.NPC;
using UnityEngine;

namespace Perspective.Simulation
{
    public class NpcCitySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private float spawnIntervalJitter = 2f;
        [SerializeField] private float spawnPointCooldown = 8f;
        [SerializeField] private int initialTrySpawn = 6;

        [Header("Spawn Table")]
        [SerializeField] private NpcSpawnTable spawnTable;

        private Dictionary<Transform, float> spawnPointCooldowns;

        private void Awake()
        {
            foreach (var e in spawnTable.entries)
            {
                e.currentCount = 0;
                e.nextPrefabIndex = 0;
            }

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
                var next = spawnInterval + Random.Range(-spawnIntervalJitter, spawnIntervalJitter);
                yield return new WaitForSeconds(Mathf.Max(0.1f, next));
            }
        }

        private void TrySpawnRandomNpc()
        {
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
            var available = spawnTable.entries.FindAll(e => e.currentCount < e.maxCount);
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
