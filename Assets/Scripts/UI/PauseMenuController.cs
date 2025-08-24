using System;
using Perspective.Input;
using UnityEngine;

namespace Perspective.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            inputReader.PauseEvent += OnPause;
            inputReader.UnPauseEvent += OnUnPause;
        }

        private void OnDisable()
        {
            inputReader.PauseEvent -= OnPause;
            inputReader.UnPauseEvent -= OnUnPause;
        }

        private void OnPause()
        {
            SetPause(true);
        }

        private void OnUnPause()
        {
            SetPause(false);
        }

        public void SetPause(bool pause)
        {
            if (!pause)
            {
                Time.timeScale = 1;
                inputReader.EnableInputBefore();
            }
            
            canvasGroup.alpha = pause ? 1 : 0;
            canvasGroup.blocksRaycasts = pause;
            canvasGroup.interactable = pause;

            if (pause)
            {
                Time.timeScale = 0f;
                inputReader.EnableUIInput();
            }
        }

        public void GoToMainMenu()
        {
        }
    }
}