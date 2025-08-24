using UnityEngine.SceneManagement;

namespace Perspective.Utils
{
    public class DataManager : Singleton<DataManager>
    {
        public int currentDay;

        public void GoNextDay()
        {
            if (currentDay == 5)
            {
                return;
            }
            currentDay++;
            SceneManager.LoadScene("MainWorld");
        }
    }
}
