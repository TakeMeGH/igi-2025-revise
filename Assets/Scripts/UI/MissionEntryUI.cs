using Perspective.Mission;
using TMPro;
using UnityEngine;

namespace Perspective.UI
{
    public class MissionEntryUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text descriptionText;

        private MissionObjective objective;

        public void Initialize(MissionObjective obj)
        {
            objective = obj;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (objective == null || descriptionText == null) return;
            
            string text = $"{objective.description}: {objective.currentCount}/{objective.requiredCount}";
            Debug.Log(text);
            if (objective.IsComplete)
                text = $"<s>{text}</s>";

            descriptionText.SetText(text);
        }
    }
}