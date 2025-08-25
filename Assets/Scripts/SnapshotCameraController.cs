using System.Collections.Generic;
using Perspective.Character.NPC;
using Perspective.Event;
using Perspective.Input;
using Perspective.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Perspective
{
    public class SnapshotCameraController : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private bool isEnabled;

        [Header("Zoom Settings")] [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minFov = 20f;
        [SerializeField] private float maxFov = 60f;
        private float targetFov;

        [Header("Input")] [SerializeField] private InputReader inputReader;
        [SerializeField] private UploadUIEvent uploadUIEvent;

        [Header("Cameras")] [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera snapshotCamera;
        private bool _isUsingCamera;

        [Header("Snapshot Output")] [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private float distanceCameraToWorld;
        [SerializeField] private RawImage snapshotDisplay;
        [SerializeField] private RectTransform frameUI;
        [SerializeField] private CanvasGroup cameraHUD;
        private readonly List<SnapshotData> _snapshotHistory = new();

        [Header("Detection")] [SerializeField] private LayerMask eventLayer;

        private void OnEnable()
        {
            inputReader.CameraEvent += UseCamera;
            inputReader.SnapshotEvent += TakeSnapshot;
            inputReader.UploadEvent += SetUploadUI;
            inputReader.ZoomCameraEvent += OnZoom;
        }

        private void OnDisable()
        {
            inputReader.CameraEvent -= UseCamera;
            inputReader.SnapshotEvent -= TakeSnapshot;
            inputReader.UploadEvent -= SetUploadUI;
            inputReader.ZoomCameraEvent -= OnZoom;
        }

        private void Start()
        {
            SetCamera(isEnabled);
            targetFov = snapshotCamera.fieldOfView;
        }

        private void Update()
        {
            if (!_isUsingCamera) return;
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            mousePos.z = distanceCameraToWorld;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            transform.position = new Vector3(mainCamera.transform.position.x,
                mainCamera.transform.position.y, mainCamera.transform.position.z);
            transform.LookAt(worldPos);

            snapshotCamera.fieldOfView = Mathf.Lerp(
                snapshotCamera.fieldOfView,
                targetFov,
                Time.deltaTime * zoomSpeed
            );
        }

        private void SetCamera(bool enableCamera)
        {
            cameraHUD.alpha = enableCamera ? 1 : 0;
            _isUsingCamera = enableCamera;

            snapshotDisplay.texture = enableCamera ? renderTexture : null;
        }

        private void UseCamera()
        {
            isEnabled = !isEnabled;
            SetCamera(isEnabled);
        }

        private void TakeSnapshot()
        {
            if (!_isUsingCamera) return;
            
            AudioManager.Instance.PlaySFX("TakePicture");

            if (renderTexture &&
                renderTexture.graphicsFormat != UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB)
            {
                renderTexture.Release();
                renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB;
                renderTexture.Create();
            }

            if (_snapshotHistory.Count >= 128)
            {
                _snapshotHistory[0].Dispose();
                _snapshotHistory.RemoveAt(0);
            }

            RenderTexture.active = renderTexture;
            var snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            snapshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            snapshot.Apply();

            RenderTexture.active = null;

            var npcCounts = CountObjectsInView();
            _snapshotHistory.Add(new SnapshotData(snapshot, npcCounts));

            Debug.Log("📸 Snapshot stored with counts!");
        }


        private Dictionary<NpcEvent, int> CountObjectsInView()
        {
            var npcCounts = new Dictionary<NpcEvent, int>();
            var totalCount = 0;

            Collider[] hits = FindObjectsByType<Collider>(FindObjectsSortMode.None);

            foreach (var hit in hits)
            {
                if ((eventLayer.value & (1 << hit.gameObject.layer)) == 0) continue;

                var viewportPos = snapshotCamera.WorldToViewportPoint(hit.transform.position);

                if (viewportPos is not { z: > 0, x: >= 0 and <= 1, y: >= 0 and <= 1 }) continue;

                totalCount++;

                var npc = hit.GetComponentInParent<NpcController>();
                if (!npc) continue;
                var npcEvent = npc.CurrentEvent;
                npcCounts.TryAdd(npcEvent, 0);
                npcCounts[npcEvent]++;
            }

            Debug.Log($"📸 Objects in snapshot: {totalCount}");
            foreach (var kvp in npcCounts)
            {
                Debug.Log($" - {kvp.Key}: {kvp.Value}");
            }

            return npcCounts;
        }

        public void SetUploadUI()
        {
            uploadUIEvent.RaiseEvent(true, _snapshotHistory);
            _snapshotHistory.Clear();
        }

        private void ClearSnapshots()
        {
            foreach (var data in _snapshotHistory)
            {
                data.Dispose();
            }

            _snapshotHistory.Clear();
        }

        private void OnZoom(float scrollValue)
        {
            if (!_isUsingCamera) return;

            targetFov -= scrollValue * zoomSpeed;
            targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
        }
    }

    [System.Serializable]
    public class SnapshotData
    {
        public Texture2D image;
        public Dictionary<NpcEvent, int> Counts;

        public SnapshotData(Texture2D image, Dictionary<NpcEvent, int> counts)
        {
            this.image = image;
            Counts = counts;
        }

        public void Dispose()
        {
            if (!image) return;
            Object.Destroy(image);
            image = null;
        }
    }
}