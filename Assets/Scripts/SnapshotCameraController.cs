using System.Collections.Generic;
using Perspective.Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Perspective
{
    public class SnapshotCameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool isEnabled;

        [Header("Input")]
        [SerializeField] private InputReader inputReader;

        [Header("Cameras")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Camera snapshotCamera;

        [Header("Snapshot Output")]
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private RawImage snapshotDisplay;
        [SerializeField] private RectTransform frameUI;
        [SerializeField] private GameObject historyImageGrid;
        private List<Texture2D> _snapshotHistory = new List<Texture2D>();


        [Header("Detection")]
        [SerializeField] private LayerMask eventLayer;

        private void OnEnable()
        {
            inputReader.CameraEvent += UseCamera;
        }

        private void OnDisable()
        {
            inputReader.CameraEvent -= UseCamera;
        }
        void Start()
        {
            SetCamera(isEnabled);
        }
        void Update()
        {   
            // Get mouse position in world space
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            mousePos.z = 10f; // ← Distance from camera to world
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            transform.position = new Vector3(mainCamera.transform.position.x, 
                mainCamera.transform.position.y, mainCamera.transform.position.z);
            transform.LookAt(worldPos);

            if (frameUI != null)
            {
                frameUI.position = mousePos;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                TakeSnapshot();
                CountObjectsInView();
            }
        }

        void SetCamera(bool enabled)
        {
            snapshotDisplay.enabled = enabled;
            frameUI.GetComponent<Image>().enabled = enabled;
        }

        void UseCamera()
        {
            isEnabled = !isEnabled;
            SetCamera(isEnabled);
        }

        void TakeSnapshot()
        {
            if (renderTexture != null && 
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
        }
        
        void CountObjectsInView()
        {
            int count = 0;

            Collider[] hits = FindObjectsOfType<Collider>();

            foreach (var hit in hits)
            {
                if ((eventLayer.value & (1 << hit.gameObject.layer)) == 0) continue;
                Debug.Log(hit.gameObject.name);
                Vector3 viewportPos = snapshotCamera.WorldToViewportPoint(hit.transform.position);

                if (viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
                {
                    count++;
                }
            }

            Debug.Log("📸 Objects in snapshot: " + count);
        }

    }
}
