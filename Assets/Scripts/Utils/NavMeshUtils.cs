using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Perspective.Utils
{
    [System.Serializable]
    public class NavMeshHotspot
    {
        public Transform region;
        [Range(0f, 10f)] public float weight = 1f;

#if UNITY_EDITOR
        public void DrawGizmo(Color color)
        {
            if (!region) return;
            Gizmos.color = color;
            Gizmos.matrix = Matrix4x4.TRS(region.position, region.rotation, region.localScale);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
#endif
    }

    public class NavMeshUtils : Singleton<NavMeshUtils>
    {
        [Header("Hotspot Settings")] [SerializeField] private List<NavMeshHotspot> hotspots = new();
        [SerializeField, Range(0f, 1f)] private float hotspotBias = 0.5f;

        /// <summary>
        /// Returns a random point on the NavMesh. May bias towards hotspot cubes if configured.
        /// </summary>
        public Vector3 GetRandomPointOnSurface(NavMeshSurface surface, int areaMask = NavMesh.AllAreas,
            int maxTries = 30)
        {
            if (!surface || !surface.navMeshData)
                return Vector3.zero;

            // Try hotspots first if enabled
            if (hotspots.Count > 0 && Random.value < hotspotBias)
            {
                Vector3 hotspotPoint = GetRandomPointInHotspot(surface, maxTries, areaMask);
                if (hotspotPoint != Vector3.zero)
                    return hotspotPoint;
            }

            // Fallback: random anywhere in navmesh bounds
            var bounds = surface.navMeshData.sourceBounds;
            for (var i = 0; i < maxTries; i++)
            {
                var randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                ) + surface.transform.position;

                if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 5f, areaMask))
                    return hit.position;
            }

            return surface.transform.position;
        }

        private Vector3 GetRandomPointInHotspot(NavMeshSurface surface, int maxTries, int areaMask)
        {
            // Pick hotspot by weight
            var totalWeight = hotspots.Sum(h => h.weight);
            if (totalWeight <= 0f) return Vector3.zero;

            var pick = Random.value * totalWeight;
            var cumulative = 0f;
            NavMeshHotspot chosen = null;
            foreach (var h in hotspots)
            {
                cumulative += h.weight;
                if (pick <= cumulative)
                {
                    chosen = h;
                    break;
                }
            }

            if (chosen == null || !chosen.region) return Vector3.zero;

            // Sample inside hotspot cube
            for (int i = 0; i < maxTries; i++)
            {
                var halfSize = chosen.region.localScale * 0.5f;
                var local = new Vector3(
                    Random.Range(-halfSize.x, halfSize.x),
                    Random.Range(-halfSize.y, halfSize.y),
                    Random.Range(-halfSize.z, halfSize.z)
                );
                var worldPos = chosen.region.position + chosen.region.rotation * local;

                if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 5f, areaMask))
                    return hit.position;
            }

            return Vector3.zero;
        }

        public bool SetDestinationNearest(NavMeshAgent agent, Vector3 targetPosition, float maxDistance = 99999f)
        {
            if (!agent) return false;
            if (!NavMesh.SamplePosition(targetPosition, out var hit, maxDistance, NavMesh.AllAreas))
                return false;

            agent.SetDestination(hit.position);
            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (hotspots == null) return;
            foreach (var h in hotspots)
                h.DrawGizmo(Color.yellow);
        }
#endif
    }
}