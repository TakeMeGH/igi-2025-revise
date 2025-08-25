using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Perspective
{
    public class OnHover_text : MonoBehaviour, IPointerEnterHandler
    {
        public TextMeshProUGUI signText;
        public string textValue;
        public Button btn;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (signText != null)
            {
                signText.text = textValue;
            }
        }

        public void OnButtonClick()
        {
            btn.interactable = false;
            StartCoroutine(EnableBtnAfterDelay(1.0f));
        }

        private IEnumerator EnableBtnAfterDelay(float delay) {
            yield return new WaitForSeconds(delay);

            btn.interactable = true;
        }
    }
}