using UnityEngine;
using System.Collections;
using StarterAssets; 
using UnityEngine.InputSystem; // NEU: Wichtig für das Input System

public class PlayerAutoStepBack : MonoBehaviour
{
    [Header("Einstellungen")]
    public float schrittDauer = 0.8f; 

    private StarterAssetsInputs _inputs;
    private PlayerInput _playerInput; // NEU: Referenz für den Hardware-Input

    void Awake()
    {
        _inputs = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>(); // Wir holen uns die Input-Komponente
    }

    public void WeicheZurueck(Transform tuerPunkt)
    {
        StopAllCoroutines(); 
        StartCoroutine(SchrittZurueckRoutine());
    }

    private IEnumerator SchrittZurueckRoutine()
    {
        if (_inputs != null)
        {
            // 1. Echten Hardware-Input blockieren, damit der Spieler nicht dazwischenfunkt
            if (_playerInput != null) _playerInput.DeactivateInput();

            // 2. Fake-Input senden: "Drücke die S-Taste"
            _inputs.move = new Vector2(0f, -1f);
            _inputs.sprint = false;

            // 3. Abwarten, bis der Schritt nach hinten fertig ist
            yield return new WaitForSeconds(schrittDauer);

            // 4. Fake-Input stoppen
            _inputs.move = Vector2.zero;
            
            // 5. Einen winzigen Moment warten, damit der Controller das Anhalten registriert
            yield return new WaitForEndOfFrame();

            // 6. Den Charakter auf der Stelle um 180 Grad drehen
            transform.Rotate(0f, 180f, 0f);

            // 7. Echten Hardware-Input wieder freigeben
            if (_playerInput != null) _playerInput.ActivateInput();
        }
        else
        {
            Debug.LogError("StarterAssetsInputs wurde auf dem Spieler nicht gefunden!");
        }
    }
}