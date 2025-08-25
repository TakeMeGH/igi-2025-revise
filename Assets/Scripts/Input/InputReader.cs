using System;
using System.Net.NetworkInformation;
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
        public Action<float> ZoomCameraEvent;
        public Action InteractEvent;
        public Action CameraEvent;
        public Action SnapshotEvent;
        public Action UploadEvent;
        public Action PauseEvent;

        #endregion

        #region UI

        public Action UnPauseEvent;
        public Action CloseUploadEvent;

        #endregion
        
        private MCInput _mcInput;
        private bool isEnableGameplayBefore;
        private bool isEnableGameplayNow;

        void OnEnable()
        {
            if (_mcInput == null)
            {
                _mcInput = new MCInput();
                _mcInput.Gameplay.SetCallbacks(this);
                _mcInput.UI.SetCallbacks(this);
            }

            EnableGameplayInput();
            isEnableGameplayBefore = true;
            isEnableGameplayNow = true;
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
            isEnableGameplayBefore = isEnableGameplayNow;
            isEnableGameplayNow = true;
        }

        public void EnableUIInput()
        {
            _mcInput.Gameplay.Disable();
            _mcInput.UI.Enable();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            isEnableGameplayBefore = isEnableGameplayNow;
            isEnableGameplayNow = false;
        }

        public void EnableInputBefore()
        {
            Debug.Log(isEnableGameplayBefore);
            if(isEnableGameplayBefore) EnableGameplayInput();
            else EnableUIInput();
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
            if (context.phase == InputActionPhase.Canceled)
            {
                CameraEvent?.Invoke();
            }
        }

        public void OnSnapshot(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Canceled)
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

        public void OnZoomCamera(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed) return;

            var value = context.ReadValue<float>();
            ZoomCameraEvent?.Invoke(value);
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                PauseEvent?.Invoke();
            }
        }

        #endregion

        #region UI

        public void OnCloseUpload(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Canceled)
            {
                CloseUploadEvent?.Invoke();
            }
        }

        public void OnUnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Canceled)
            {
                UnPauseEvent?.Invoke();
            }
        }

        #endregion
    }
}