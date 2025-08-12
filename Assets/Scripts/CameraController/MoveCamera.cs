using UnityEngine;

namespace Perspective.CameraController
{
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void Update()
        {
            transform.position = target.position;
        }
    }
}
