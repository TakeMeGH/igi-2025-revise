using Perspective.Input;
using Perspective.Simulation;
using UnityEngine;
using UnityEngine.Playables;

namespace Perspective.Utils
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private GameObject graffiti;
        [SerializeField] private GameObject grafitiBoy;
        [SerializeField] private GameObject[] beggars;
        [SerializeField] private GameObject[] cars;

        #region StartGame

        [SerializeField] private GameObject cameraHolder;
        [SerializeField] private GameObject player;
        [SerializeField] private NpcCitySpawner spawner;
        [SerializeField] private InputReader inputReader;

        #endregion

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

            switch (currentDay)
            {
                case 1:
                    PlayTimeline("Day1/TL_Day1");
                    inputReader.EnableUIInput();
                    AudioManager.Instance.PlayMusic("Day1BGM");
                    break;
                case 2:
                    PlayTimeline("Day2/TL_Day2");
                    inputReader.EnableUIInput();
                    AudioManager.Instance.PlayMusic("Day2BGM");
                    break;
                case 3:
                    // PlayTimeline("Day3/TL_Day3");
                    PlayTimeline("Day3/TL_Day3pt2");
                    inputReader.EnableUIInput();
                    AudioManager.Instance.PlayMusic("Day3BGM");
                    break;
                default:
                    StartGame();
                    break;
            }
        }

        public void PlayTimeline(string path)
        {
            var asset = Resources.Load<PlayableAsset>(path);

            if (asset)
            {
                director.playableAsset = asset;
                director.time = 0;
                director.Play();
            }
            else
            {
                Debug.LogError("Timeline not found at: " + path);
            }
        }

        public void StartGame()
        {
            cameraHolder.SetActive(true);
            player.SetActive(true);
            spawner.StartSpawner();
            inputReader.EnableGameplayInput();
        }

        public void TransitionNextDay()
        {
            PlayTimeline("TL_Transition");
        }

        public void GoNextDay()
        {
            if (DataManager.Instance.currentDay == 5)
            {
                cameraHolder.SetActive(false);
                player.SetActive(false);
                inputReader.EnableUIInput();
                PlayTimeline("Day5/TL_Day5");
                // PlayTimeline("Day5/TL_Day5pt2");
            }
            else
            {
                DataManager.Instance.GoNextDay();
            }
        }

        public void FinishGame()
        {
            Debug.Log("Finish Game");
        }
    }
}