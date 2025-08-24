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
            SceneManager.LoadScene("MainWorld");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
