using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCarController : MonoBehaviour
{
    public float acceleration = 4f;
    public float braking = 12f;
    public float maxForwardSpeed = 15f;
    public float maxReverseSpeed = 4f;
    public float turnSpeed = 40f;

    [HideInInspector] public bool canDrive = false;

    private Rigidbody rb;
    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.8f, 0f);
    }

    void Update()
    {
        if (!canDrive) return;

        float moveInput = 0f;
        float turnInput = 0f;

        // WASD
        if (Keyboard.current.wKey.isPressed) moveInput += 1f;
        if (Keyboard.current.sKey.isPressed) moveInput -= 1f;
        if (Keyboard.current.aKey.isPressed) turnInput -= 1f;
        if (Keyboard.current.dKey.isPressed) turnInput += 1f;

        // Pfeiltasten
        if (Keyboard.current.upArrowKey.isPressed) moveInput += 1f;
        if (Keyboard.current.downArrowKey.isPressed) moveInput -= 1f;
        if (Keyboard.current.leftArrowKey.isPressed) turnInput -= 1f;
        if (Keyboard.current.rightArrowKey.isPressed) turnInput += 1f;

        HandleAcceleration(moveInput);
        HandleSteering(turnInput);

        StabilizeCar();
    }

    void HandleAcceleration(float input)
    {
        float targetSpeed = 0f;

        if (input > 0)
            targetSpeed = maxForwardSpeed;
        else if (input < 0)
            targetSpeed = -maxReverseSpeed;

        if (input != 0)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0,
                braking * Time.fixedDeltaTime
            );
        }

        Vector3 velocity = transform.forward * currentSpeed;
        velocity.y = rb.linearVelocity.y;

        rb.linearVelocity = velocity;
    }

    void HandleSteering(float input)
    {
        float direction = Mathf.Sign(currentSpeed);
        float turn = input * turnSpeed * direction * Time.fixedDeltaTime;

        Quaternion rotation = Quaternion.Euler(0, turn, 0);
        rb.MoveRotation(rb.rotation * rotation);
    }

    void StabilizeCar()
    {
        Vector3 tilt = transform.eulerAngles;

        if (tilt.x > 180) tilt.x -= 360;
        if (tilt.z > 180) tilt.z -= 360;

        float stabilizingForce = 2f;

        Vector3 torque = new Vector3(
            -tilt.x * stabilizingForce,
            0f,
            -tilt.z * stabilizingForce
        );

        rb.AddTorque(torque);
    }
}