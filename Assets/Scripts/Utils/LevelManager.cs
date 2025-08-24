using UnityEngine;

namespace Perspective.Utils
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject graffiti;
        [SerializeField] private GameObject grafitiBoy;
        [SerializeField] private GameObject[] beggars;
        [SerializeField] private GameObject[] cars;

        private int currentDay;

        private void Start()
        {
            currentDay = DataManager.Instance.currentDay;

            HandleDayLogic();
        }

        private void HandleDayLogic()
        {
            var beggarsActive = currentDay is 1 or 2;
            foreach (var beggar in beggars)
            {
                if (beggar) beggar.SetActive(beggarsActive);
            }

            var graffitiActive = currentDay == 3 || currentDay == 4;
            if (graffiti) graffiti.SetActive(graffitiActive);
            if (grafitiBoy) grafitiBoy.SetActive(graffitiActive);

            if (currentDay is >= 1 and <= 4)
            {
                foreach (var car in cars)
                {
                    if (!car) continue;
                    car.SetActive(Random.value > 0.5f);
                }
            }
            else
            {
                foreach (var car in cars)
                    if (car)
                        car.SetActive(false);
            }
        }
    }
}