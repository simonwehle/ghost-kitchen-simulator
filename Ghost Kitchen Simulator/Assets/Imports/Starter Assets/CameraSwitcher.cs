using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using StarterAssets;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera fpCamera;
    public CinemachineCamera tpCamera; // Zuweisung der Third Person Kamera im Inspector
    private ThirdPersonController _controller;
    public GameObject _player_cam_root;
    public GameObject _player_arm;
    private bool _isFirstPerson = false;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minDistance = 1f;
    public float maxDistance = 6f;
    private CinemachineThirdPersonFollow _tpFollow;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        
        // Holen der Follow-Komponente der Third Person Kamera
        if (tpCamera != null)
        {
            _tpFollow = tpCamera.GetComponent<CinemachineThirdPersonFollow>();
        }
    }

    void Update()
    {
        // Kamera wechseln
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            _isFirstPerson = !_isFirstPerson;
            fpCamera.Priority = _isFirstPerson ? 20 : 5;
        }

        // Zoom-Logik (nur wenn nicht in First Person)
        if (!_isFirstPerson && _tpFollow != null)
        {
            HandleZoom();
        }

        if (_isFirstPerson)
        {
            SyncRotation();
        }
    }

    void HandleZoom()
    {
        // Mausrad-Input abfragen
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)
        {
            // Neuen Abstand berechnen (scroll ist meist 120 oder -120, daher Normalisierung)
            float newDistance = _tpFollow.CameraDistance - (scroll * 0.01f * zoomSpeed);
            
            // Wert zwischen den Grenzwerten halten
            _tpFollow.CameraDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        }
    }

    void SyncRotation()
    {
        float yaw = _player_cam_root.transform.eulerAngles.y;
        _player_arm.transform.rotation = Quaternion.Euler(0, yaw, 0);
    }
}