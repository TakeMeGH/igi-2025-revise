using System.Collections.Generic;
using Perspective.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Perspective
{
    public class SnapshotCameraController : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private bool isEnabled;

        [Header("Input")] [SerializeField] private InputReader inputReader;

        [Header("Cameras")] [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera snapshotCamera;
        private bool _isUsingCamera;

        [Header("Snapshot Output")] [SerializeField]
        private RenderTexture renderTexture;

        [SerializeField] private RawImage snapshotDisplay;
        [SerializeField] private RectTransform frameUI;
        [SerializeField] private GameObject historyImageGrid;
        private readonly List<Texture2D> _snapshotHistory = new();


        [Header("Detection")] [SerializeField] private LayerMask eventLayer;

        private void OnEnable()
        {
            inputReader.CameraEvent += UseCamera;
            inputReader.SnapshotEvent += TakeSnapshot;
        }

        private void OnDisable()
        {
            inputReader.CameraEvent -= UseCamera;
            inputReader.SnapshotEvent -= TakeSnapshot;
        }

        private void Start()
        {
            SetCamera(isEnabled);
        }

        private void Update()
        {
            if (!_isUsingCamera) return;
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            mousePos.z = 10f; // ← Distance from camera to world
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            transform.position = new Vector3(mainCamera.transform.position.x,
                mainCamera.transform.position.y, mainCamera.transform.position.z);
            transform.LookAt(worldPos);
        }

        private void SetCamera(bool enableCamera)
        {
            snapshotDisplay.enabled = enableCamera;
            frameUI.GetComponent<Image>().enabled = enableCamera;
            _isUsingCamera = enableCamera;
        }

        private void UseCamera()
        {
            isEnabled = !isEnabled;
            SetCamera(isEnabled);
        }

        private void TakeSnapshot()
        {
            if (!_isUsingCamera) return;
            if (renderTexture &&
                renderTexture.graphicsFormat != UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB)
            {
                renderTexture.Release();
                renderTexture.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB;
                renderTexture.Create();
            }

            // Render camera to texture
            snapshotCamera.targetTexture = renderTexture;
            snapshotCamera.Render();

            // Copy to Texture2D
            RenderTexture.active = renderTexture;
            Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            snapshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            snapshot.Apply();

            RenderTexture.active = null;
            snapshotCamera.targetTexture = null;

            // Show in UI
            snapshotDisplay.texture = snapshot;

            _snapshotHistory.Add(snapshot);

            historyImageGrid.transform.GetChild(_snapshotHistory.Count - 1).GetComponent<RawImage>().texture = snapshot;

            Debug.Log("📸 Snapshot taken!");
            CountObjectsInView();
        }

        private void CountObjectsInView()
        {
            var count = 0;

            Collider[] hits = FindObjectsByType<Collider>(FindObjectsSortMode.None);

            foreach (var hit in hits)
            {
                if ((eventLayer.value & (1 << hit.gameObject.layer)) == 0) continue;
                Debug.Log(hit.gameObject.name);
                Vector3 viewportPos = snapshotCamera.WorldToViewportPoint(hit.transform.position);

                if (viewportPos is { z: > 0, x: >= 0 and <= 1, y: >= 0 and <= 1 })
                {
                    count++;
                }
            }

            Debug.Log("📸 Objects in snapshot: " + count);
        }
    }
}