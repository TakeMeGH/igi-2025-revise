
using UnityEngine;

namespace Perspective
{
    public class ToggleTutorial : MonoBehaviour
    {
        public CanvasGroup toOpen;
        public CanvasGroup toClose;

        public void OnButtonClick()
        {
            print("kepencet");
            toOpen.alpha = 1f;
            toOpen.interactable = true;
            toOpen.blocksRaycasts = true;

            if (toClose != null)
            {
                toClose.alpha = 0f;
                toClose.interactable = false;
                toClose.blocksRaycasts = false;
            }
        }
    }
}
