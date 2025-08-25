using System;
using Perspective.Input;
using Perspective.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Perspective
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] InputReader inputReader;

        private void Start()
        {
            inputReader.EnableUIInput();
            AudioManager.Instance.PlayMusic("MainMenuBGM");
        }

        public void GotoInGame()
        {
            DataManager.Instance.currentDay = 1;
            DataManager.Instance.reputation = 5;
            SceneManager.LoadScene("MainWorld");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
