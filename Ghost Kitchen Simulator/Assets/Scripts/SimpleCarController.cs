using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f;

    [HideInInspector]
    public bool canDrive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!canDrive) return;

        float move = 0f;
        float turn = 0f;

        // WASD
        if (Keyboard.current.wKey.isPressed) move += 0.7f;
        if (Keyboard.current.sKey.isPressed) move -= 0.2f;
        if (Keyboard.current.aKey.isPressed) turn -= 0.5f;
        if (Keyboard.current.dKey.isPressed) turn += 0.2f;

        // Pfeiltasten
        if (Keyboard.current.upArrowKey.isPressed) move += 0.7f;
        if (Keyboard.current.downArrowKey.isPressed) move -= 0.5f;
        if (Keyboard.current.leftArrowKey.isPressed) turn -= 0.2f;
        if (Keyboard.current.rightArrowKey.isPressed) turn += 0.2f;

        move *= speed * Time.deltaTime;
        turn *= rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, move);
        transform.Rotate(0, turn, 0);
    }
}
