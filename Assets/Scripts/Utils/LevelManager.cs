using Perspective.Input;
using Perspective.Interactions;
using Perspective.Interactions.Core;
using Perspective.Simulation;
using Perspective.UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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
        [SerializeField] private CanvasGroup missionController;
        [SerializeField] private Bed bed;

        #endregion

        private int currentDay;

        private Interactor _interactor;

        private void Start()
        {
            currentDay = DataManager.Instance.currentDay;
            HandleDayLogic();
            _interactor = FindAnyObjectByType<Interactor>();
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

            // ✅ Use cached interactor
            _interactor?.DisableInteractor();
            bed.SetIsInteractable(false);

            switch (currentDay)
            {
                case 1:
                    PlayTimeline("Day1/TL_Day1");
                    inputReader.EnableUIInput();
                    missionController.alpha = 0;
                    AudioManager.Instance.PlayMusic("Day1BGM");
                    break;
                case 2:
                    PlayTimeline("Day2/TL_Day2");
                    inputReader.EnableUIInput();
                    missionController.alpha = 0;
                    AudioManager.Instance.PlayMusic("Day2BGM");
                    break;
                case 3:
                    if (DataManager.Instance.GetReputation() > 0) PlayTimeline("Day3/TL_Day3");
                    else if (DataManager.Instance.GetReputation() < 0) PlayTimeline("Day3/TL_Day3pt2");
                    inputReader.EnableUIInput();
                    missionController.alpha = 0;
                    AudioManager.Instance.PlayMusic("Day3BGM");
                    break;
                case 4:
                    PlayTimeline("Day4/TL_Day4");
                    inputReader.EnableUIInput();
                    missionController.alpha = 0;
                    AudioManager.Instance.PlayMusic("Day4BGM");
                    break;
                case 5:
                    PlayTimeline("Day5/TL_Day5Start");
                    inputReader.EnableUIInput();
                    missionController.alpha = 0;
                    AudioManager.Instance.PlayMusic("Day5BGM");
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
            missionController.alpha = 1;
            inputReader.EnableGameplayInput();

            // ✅ Use cached interactor
            _interactor?.EnableInteractor();
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
                missionController.alpha = 0;

                if (DataManager.Instance.GetReputation() > 0) PlayTimeline("Day5/TL_Day5");
                else if (DataManager.Instance.GetReputation() < 0) PlayTimeline("Day5/TL_Day5pt2");
                
                _interactor?.DisableInteractor();
            }
            else
            {
                DataManager.Instance.GoNextDay();
            }
        }

        public void FinishGame()
        {
            GoToMainMenu();
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}