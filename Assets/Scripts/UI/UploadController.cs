using System.Collections.Generic;
using Perspective.Event;
using Perspective.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Perspective.UI
{
    public class UploadController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        [SerializeField] private CanvasGroup uploadHUDCanvas;
        [SerializeField] private UploadUIEvent uploadUIEvent;

        [SerializeField] private GameObject listPhotoParent;
        [SerializeField] private RawImage highlightPhoto;

        [SerializeField] private Button uploadButton;
        [SerializeField] private TMP_InputField description;

        [SerializeField] private SosmedController sosmedController;

        private int _currentPhotoIndex;
        private readonly List<SnapshotData> _snapshotHistory = new();

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
            uploadHUDCanvas.alpha = isUsingUI ? 1 : 0;
            uploadHUDCanvas.blocksRaycasts = isUsingUI;
            uploadHUDCanvas.interactable = isUsingUI;

            if (!isUsingUI)
            {
                return;
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
            highlightPhoto.texture = listPhotoParent.transform.GetChild(index).GetComponent<RawImage>().texture;
            _currentPhotoIndex = index;
        }

        private void OnUpload()
        {
            sosmedController.SetController(true, _snapshotHistory[_currentPhotoIndex], description.text);
            listPhotoParent.transform.GetChild(_currentPhotoIndex).gameObject.SetActive(false);
        }
        
        private void OnCloseUploadMenu()
        {
            SetUploadUI(false, null);
            inputReader.EnableGameplayInput();
        }

    }
}