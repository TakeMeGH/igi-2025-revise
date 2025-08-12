using Perspective.Input;
using UnityEngine;

namespace Perspective.Character.Player
{
    public class LookController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform orientationTransform;
        [SerializeField] private float sensitivityY = 10f;
        [SerializeField] private float sensitivityX = 10f;
        private float _yRotation;
        private float _xRotation;
        private void OnEnable()
        {
            inputReader.LookEvent += PlayerLook;
        }

        private void OnDisable()
        {
            inputReader.LookEvent -= PlayerLook;
        }

        private void PlayerLook(Vector2 lookValue)
        {
            _yRotation += lookValue.x * Time.deltaTime * sensitivityY;
            _xRotation -= lookValue.y * Time.deltaTime * sensitivityX;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            cameraTransform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            orientationTransform.localRotation = Quaternion.Euler(0, _yRotation, 0);
        }
    }
}
