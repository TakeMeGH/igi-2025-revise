using System;
using Perspective.Input;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Perspective.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CanvasGroup canvasGroup;
        [FormerlySerializedAs("PauseImage")] [SerializeField] private Image pauseImage;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Sprite defaultPause;
        [SerializeField] private Sprite internetConnectionPause;

        private bool pauseConditions = false;

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
            SetPause(!pauseConditions);
        }

        private void OnUnPause()
        {
            if (pauseImage.sprite == internetConnectionPause) return;
            SetPause(!pauseConditions);
        }

        public void SetPause(bool pause)
        {
            pauseConditions = pause;
            if (!pause)
            {
                Time.timeScale = 1;
                inputReader.EnableInputBefore();
                pauseImage.sprite = defaultPause;

                pauseButton.gameObject.SetActive(true);
                pauseImage.SetNativeSize();
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

        public void PauseInternetConnections()
        {
            pauseImage.sprite = internetConnectionPause;
            pauseImage.SetNativeSize();
            pauseButton.gameObject.SetActive(false);
            SetPause(true);
        }

        public void GoToMainMenu()
        {
        }
    }
}