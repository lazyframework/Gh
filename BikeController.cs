using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk Input System baru

public class BikeController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;
    public float maxSpeed = 15f;

    [Header("Pedals")]
    public Transform crank;
    public float pedalSpeedMultiplier = 40f;

    [Header("References")]
    public Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // --- BACA INPUT DENGAN SISTEM BARU ---
        horizontalInput = 0f;
        verticalInput = 0f;

        // Cek apakah ada keyboard yang terhubung
        if (Keyboard.current != null)
        {
            // Input Vertikal (Maju/Mundur) pakai W/S atau Panah Atas/Bawah
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                verticalInput = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                verticalInput = -1f;

            // Input Horizontal (Belok) pakai A/D atau Panah Kiri/Kanan
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1f;
        }

        // Putar crank sesuai input maju/mundur
        if (crank != null && Mathf.Abs(verticalInput) > 0.05f)
        {
            float pedalRotation = verticalInput * pedalSpeedMultiplier * Time.deltaTime;
            crank.Rotate(Vector3.right * pedalRotation); // Kalau arahnya terbalik, ganti jadi Vector3.left
        }
    }

    void FixedUpdate()
    {
        // Gerakan maju
        Vector3 moveForce = transform.forward * verticalInput * moveSpeed;
        rb.AddForce(moveForce, ForceMode.Acceleration);

        // Batasi kecepatan
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Belok
        if (Mathf.Abs(horizontalInput) > 0.05f)
        {
            float turn = horizontalInput * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }
    }
}
