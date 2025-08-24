using UnityEngine;
using UnityEngine.SceneManagement;

namespace Perspective.Utils
{
    public class DataManager : Singleton<DataManager>
    {
        public int currentDay;
        public int reputation;

        public void GoNextDay()
        {
            if (currentDay == 5)
            {
                return;
            }

            currentDay++;
            SceneManager.LoadScene("MainWorld");
        }

        public void AddReputation(int value)
        {
            reputation += value;
            reputation = Mathf.Clamp(value, 0, 10);
        }

        public int GetReputation()
        {
            if (reputation > 5) return 1;
            else return -1;
        }
    }
}