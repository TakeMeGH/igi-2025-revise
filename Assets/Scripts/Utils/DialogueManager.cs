using UnityEngine;
using Yarn.Unity;

namespace Perspective.Utils
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private DialogueRunner dialogueRunner;
        private LevelManager levelManager;
        private int counter;

        public void Start()
        {
            dialogueRunner = FindAnyObjectByType<DialogueRunner>();
            levelManager = FindAnyObjectByType<LevelManager>();

            dialogueRunner.onDialogueComplete?.AddListener(StartAnotherActions);
        }

        public void StartDialogue(string node)
        {
            counter++;
            dialogueRunner.StartDialogue(node);
        }

        void CallDialogue()
        {
            StartDialogue("YS_Day5_2pt2");
        }

        private void StartAnotherActions()
        {
            switch (DataManager.Instance.currentDay)
            {
                case 1:
                    if (counter == 1) levelManager.PlayTimeline("Day1/TL_Day1_2");
                    else if (counter == 2) levelManager.PlayTimeline("Day1/TL_Day1_3");
                    break;
                case 2:
                    if (counter == 1) levelManager.PlayTimeline("Day2/TL_Day2_2");
                    break;
                case 3:
                    if (DataManager.Instance.GetReputation() > 0) levelManager.PlayTimeline("Day3/TL_Day3_2");
                    else if (DataManager.Instance.GetReputation() < 0) levelManager.PlayTimeline("Day3/TL_Day3_2pt2");
                    break;
                case 5:
                    if (DataManager.Instance.GetReputation() > 0)
                    {
                        if (counter == 1) levelManager.PlayTimeline("Day5/TL_Day5_2");
                        if (counter == 2) levelManager.PlayTimeline("TL_TransitionFinishGame");
                    }
                    else if (DataManager.Instance.GetReputation() < 0)
                    {
                        if (counter == 1) Invoke(nameof(CallDialogue), 0.1f);
                        if (counter == 2) levelManager.PlayTimeline("Day5/TL_Day5_2pt2");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}