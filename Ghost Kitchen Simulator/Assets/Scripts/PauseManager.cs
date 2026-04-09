using UnityEngine;
using UnityEngine.InputSystem;
// WICHTIG: Füge diesen Namespace hinzu, damit das Script die Starter Assets findet
using StarterAssets; 

public class PauseManager : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseHint;

    [Header("Starter Assets Reference")]
    // Hier ziehst du deinen Player (der die Komponente StarterAssetsInputs hat) rein
    [SerializeField] private StarterAssetsInputs playerInputs;

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseHint.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Maus wieder einsperren
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Input für Kamera und Bewegung wieder freigeben
        if (playerInputs != null)
        {
            playerInputs.cursorLocked = true;
            playerInputs.cursorInputForLook = true;
        }
    }

    void Pause()
    {
        pauseHint.SetActive(false); 
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Maus befreien
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Input für Kamera blockieren, damit sie sich im Menü nicht mitdreht
        if (playerInputs != null)
        {
            playerInputs.cursorLocked = false;
            playerInputs.cursorInputForLook = false;
            
            // Optional: Bewegung auf Null setzen, damit der Charakter nicht weiterläuft
            playerInputs.move = Vector2.zero;
            playerInputs.look = Vector2.zero;
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}