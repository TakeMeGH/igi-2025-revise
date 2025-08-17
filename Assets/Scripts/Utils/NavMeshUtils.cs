using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Perspective.Utils
{
    public static class NavMeshUtils
    {
        public static Vector3 GetRandomPointOnSurface(NavMeshSurface surface, int areaMask = NavMesh.AllAreas,
            int maxTries = 30)
        {
            var bounds = surface.navMeshData.sourceBounds;

            for (int i = 0; i < maxTries; i++)
            {
                var randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );

                randomPos += surface.transform.position;

                if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 5f, areaMask))
                {
                    return hit.position;
                }
            }

            return surface.transform.position;
        }

        public static bool SetDestinationNearest(NavMeshAgent agent, Vector3 targetPosition, float maxDistance = 99999f)
        {
            if (!NavMesh.SamplePosition(targetPosition, out var hit, maxDistance, NavMesh.AllAreas)) return false;
            agent.SetDestination(hit.position);
            return true;
        }
    }
}