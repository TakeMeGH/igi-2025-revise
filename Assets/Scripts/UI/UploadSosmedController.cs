using System;
using System.Collections;
using System.Collections.Generic;
using Perspective.Character.NPC;
using Perspective.Event;
using Perspective.Input;
using Perspective.Mission;
using Perspective.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Perspective.UI
{
    public class UploadSosmedController : MonoBehaviour
    {
        #region Upload

        [SerializeField] private InputReader inputReader;

        [SerializeField] private CanvasGroup uploadHUDCanvas;
        [SerializeField] private UploadUIEvent uploadUIEvent;

        [SerializeField] private GameObject listPhotoParent;
        [SerializeField] private RawImage highlightPhoto;

        [SerializeField] private Button uploadButton;
        [SerializeField] private TMP_InputField descriptionField;

        private int _currentPhotoIndex;
        private readonly List<SnapshotData> _snapshotHistory = new();
        private bool _enablePhotoClick;

        #endregion

        #region Sosmed

        [SerializeField] private ScrollRect commentsRect;
        [SerializeField] private GameObject commentPrefab;

        #endregion


        private void OnEnable()
        {
            uploadUIEvent.EventAction += SetUploadUI;
            uploadButton.onClick.AddListener(OnUpload);
            inputReader.CloseUploadEvent += OnCloseUploadMenu;
        }

        private void OnDisable()
        {
            uploadUIEvent.EventAction -= SetUploadUI;
            uploadButton.onClick.RemoveListener(OnUpload);
            inputReader.CloseUploadEvent = OnCloseUploadMenu;
        }

        private void SetUploadUI(bool isUsingUI, List<SnapshotData> snapshotHistory)
        {
            _enablePhotoClick = isUsingUI;
            uploadHUDCanvas.alpha = isUsingUI ? 1 : 0;
            uploadHUDCanvas.blocksRaycasts = isUsingUI;
            uploadHUDCanvas.interactable = isUsingUI;
            highlightPhoto.texture = null;

            if (!isUsingUI)
            {
                return;
            }

            foreach (var snapshotData in _snapshotHistory)
            {
                snapshotData.Dispose();
            }

            _snapshotHistory.Clear();
            _snapshotHistory.AddRange(snapshotHistory);

            inputReader.EnableUIInput();

            for (var i = 0; i < listPhotoParent.transform.childCount; i++)
            {
                var index = i;

                if (i < snapshotHistory.Count)
                {
                    listPhotoParent.transform.GetChild(i).gameObject.SetActive(true);
                    listPhotoParent.transform.GetChild(i).GetComponent<RawImage>().texture = snapshotHistory[i].image;
                    listPhotoParent.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                    {
                        OnPhotoClicked(index);
                    });
                }
                else
                {
                    listPhotoParent.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    listPhotoParent.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        private void OnPhotoClicked(int index)
        {
            if (!_enablePhotoClick) return;
            highlightPhoto.texture = listPhotoParent.transform.GetChild(index).GetComponent<RawImage>().texture;
            _currentPhotoIndex = index;
        }

        private void OnUpload()
        {
            _enablePhotoClick = false;
            descriptionField.interactable = false;
            GoUpload(_snapshotHistory[_currentPhotoIndex], descriptionField.text);
            listPhotoParent.transform.GetChild(_currentPhotoIndex).gameObject.SetActive(false);
        }

        public void OnCloseUploadMenu()
        {
            SetUploadUI(false, null);
            inputReader.EnableGameplayInput();
        }

        public void GoUpload(SnapshotData postImage, string descriptionText)
        {
            if (commentsRect && commentsRect.content)
            {
                foreach (Transform child in commentsRect.content)
                {
                    if (child)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
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
                    var stance = await ChatBotUtils.Instance.AskBot(stancePrompt, descriptionField.text);

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

                var answer = await ChatBotUtils.Instance.AskBot(commentsPrompt, descriptionField.text);
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

            descriptionField.interactable = true;
            _enablePhotoClick = true;
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
    }
}