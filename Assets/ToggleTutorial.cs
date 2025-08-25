
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Perspective
{
    public class ToggleTutorial : MonoBehaviour
    {
        public string toOpen;

        public void OnButtonClick()
        {
            SceneManager.LoadScene(toOpen);
        }
    }
}
