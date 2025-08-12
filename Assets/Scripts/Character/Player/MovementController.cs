using Perspective.Input;
using UnityEngine;

namespace Perspective.Character.Player
{
    public class MovementController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float groundDrag = 10f;

        [SerializeField] private Transform orientation;
        private Vector2 _rawInput;
        private Rigidbody _rigidbody;
        
        [Header("Ground Check")]
        [SerializeField] private float groundCheckDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;
        private bool _isGrounded;

        private void OnEnable()
        {
            inputReader.MoveEvent += MoveInput;
        }

        private void OnDisable()
        {
            inputReader.MoveEvent -= MoveInput;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            CheckGround();
            SpeedControl();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void MoveInput(Vector2 moveValue)
        {
            _rawInput = moveValue;
        }

        private void CheckGround()
        {
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
        }

        private void Move()
        {
            _rigidbody.linearDamping = _isGrounded ? groundDrag : 0.0f;
            
            var moveDirection = orientation.forward * _rawInput.y + orientation.right * _rawInput.x;
            _rigidbody.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        }

        private void SpeedControl()
        {
            var flatVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            if (!(flatVelocity.magnitude > moveSpeed)) return;
            var limitedVelocity = flatVelocity.normalized * moveSpeed;
            _rigidbody.linearVelocity = new Vector3(limitedVelocity.x, _rigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }
}