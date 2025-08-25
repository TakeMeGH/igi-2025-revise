using System;
using System.Collections;
using System.Collections.Generic;
using Perspective.Character.NPC;
using Perspective.Mission;
using Perspective.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Perspective.UI
{
    public class SosmedController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RawImage highlight;
        [SerializeField] private TMP_Text description;

        [SerializeField] private ScrollRect commentsRect;
        [SerializeField] private GameObject commentPrefab;
        [SerializeField] private Button doneButton;

        public void SetController(bool enable, SnapshotData postImage, string descriptionText)
        {
            canvasGroup.alpha = enable ? 1 : 0;
            canvasGroup.interactable = enable;
            canvasGroup.blocksRaycasts = enable;

            if (enable) doneButton.onClick.AddListener(DoneButtonClick);
            else doneButton.onClick.RemoveListener(DoneButtonClick);

            if (!enable) return;

            // Bersihin isi scroll content dulu biar gak numpuk
            foreach (Transform child in commentsRect.content)
            {
                Destroy(child.gameObject);
            }

            highlight.texture = postImage.image;
            description.text = descriptionText;
            CreateComments(GetUsedEvent(postImage.Counts));
        }

        private async void CreateComments(NpcEvent currentEvent)
        {
            try
            {
                string commentsPrompt;

                if (currentEvent == NpcEvent.None)
                {
                    commentsPrompt = ChatBotUtils.Instance.BuildNoneEventCommentsPrompt();
                }
                else
                {
                    var stancePrompt = ChatBotUtils.Instance.BuildStancePrompt(currentEvent);
                    var stance = await ChatBotUtils.Instance.AskBot(stancePrompt, description.text);

                    Debug.Log($"Stance : {stance} ---- Event : {currentEvent}");

                    commentsPrompt = stance == "tidak relevan"
                        ? ChatBotUtils.Instance.BuildNoneEventCommentsPrompt()
                        : ChatBotUtils.Instance.BuildCommentsPrompt(stance);

                    if (stance == "pro rakyat")
                    {
                        DataManager.Instance.AddReputation(-1);
                    }
                    else if (stance == "pro pemerintah")
                    {
                        DataManager.Instance.AddReputation(1);
                    }
                }

                var answer = await ChatBotUtils.Instance.AskBot(commentsPrompt, description.text);
                var comments = ChatBotUtils.Instance.FormatComments(answer);

                FindAnyObjectByType<MissionManager>().OnPhotoTaken(currentEvent);
                Debug.Log(answer);

                StartCoroutine(SpawnCommentsWithInterval(comments, 0.5f));
            }
            catch (Exception e)
            {
                Debug.LogError($"[SosmedController] Failed to create comments: {e.Message}");
            }
        }

        private IEnumerator SpawnCommentsWithInterval(List<string> comments, float interval)
        {
            foreach (var comment in comments)
            {
                var newComment = Instantiate(commentPrefab, commentsRect.content);

                var commentController = newComment.GetComponent<CommentController>();
                if (commentController) commentController.SetCommentText(comment);
                yield return new WaitForSeconds(interval);
            }

            doneButton.interactable = true;
        }

        private static NpcEvent GetUsedEvent(Dictionary<NpcEvent, int> counts)
        {
            var selectedEvent = NpcEvent.None;
            var maxCount = 0;
            foreach (var count in counts)
            {
                if (count.Key == NpcEvent.None || count.Key == NpcEvent.DisableEvent) continue;
                if (count.Value <= maxCount) continue;
                selectedEvent = count.Key;
                maxCount = count.Value;
            }

            return selectedEvent;
        }

        private void DoneButtonClick()
        {
            doneButton.interactable = false;
            SetController(false, null, null);
        }
    }
}