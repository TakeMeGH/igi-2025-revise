using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Perspective.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
    public class InputReader : ScriptableObject, MCInput.IGameplayActions, MCInput.IUIActions
    {
        #region Gameplay

        public Action<Vector2> MoveEvent;
        public Action<Vector2> LookEvent;
        public Action InteractEvent;
        public Action CameraEvent;
        public Action SnapshotEvent;
        public Action UploadEvent;

        #endregion

        #region UI

        public Action UnPausePerformed;
        public Action CloseUploadEvent;

        #endregion


        private MCInput _mcInput;

        void OnEnable()
        {
            if (_mcInput == null)
            {
                _mcInput = new MCInput();
                _mcInput.Gameplay.SetCallbacks(this);
                _mcInput.UI.SetCallbacks(this);
            }

            EnableGameplayInput();
        }

        void OnDisable()
        {
            if (_mcInput != null) _mcInput.Gameplay.Disable();
        }

        public void EnableGameplayInput()
        {
            _mcInput.Gameplay.Enable();
            _mcInput.UI.Disable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void EnableUIInput()
        {
            _mcInput.Gameplay.Disable();
            _mcInput.UI.Enable();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        #region Gameplay

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                InteractEvent?.Invoke();
            }
        }

        public void OnCamera(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                CameraEvent?.Invoke();
            }
        }

        public void OnSnapshot(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SnapshotEvent?.Invoke();
            }
        }

        public void OnUpload(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                UploadEvent?.Invoke();
            }
        }

        #endregion

        #region UI

        public void OnCloseUpload(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                CloseUploadEvent?.Invoke();
            }
        }

        #endregion
    }
}