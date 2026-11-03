using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 500f; // Pakai AddForce, jadi angkanya harus besar
    public float turnSpeed = 100f;
    public float maxSpeed = 10f;

    [Header("Pedal Settings")]
    public Transform crank;
    public float pedalSpeedMultiplier = 100f;

    [Header("Physics")]
    public Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        
        // Opsional: Supaya sepeda tidak gampang jatuh miring
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Update()
    {
        horizontalInput = 0f;
        verticalInput = 0f;

        // --- INPUT KEYBOARD (WASD / Panah) ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) verticalInput = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) verticalInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;
        }

        // --- INPUT GAMEPAD FISIK ---
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Vector2 stick = gamepad.leftStick.ReadValue();
            if (Mathf.Abs(stick.x) > 0.1f) horizontalInput = stick.x;
            if (Mathf.Abs(stick.y) > 0.1f) verticalInput = stick.y;
        }

        // --- PUTAR PEDAL ---
        if (crank != null && Mathf.Abs(verticalInput) > 0.05f)
        {
            crank.Rotate(Vector3.right * verticalInput * pedalSpeedMultiplier * Time.deltaTime);
        }

        // --- BELOK (Rotasi Rigidbody) ---
        if (Mathf.Abs(horizontalInput) > 0.05f)
        {
            float turn = horizontalInput * turnSpeed * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // --- MAJU / MUNDUR (Pakai Fisika AddForce) ---
        if (Mathf.Abs(verticalInput) > 0.05f)
        {
            Vector3 moveForce = transform.forward * verticalInput * moveSpeed;
            rb.AddForce(moveForce, ForceMode.Force);
        }

        // Batasi kecepatan maksimal
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}
