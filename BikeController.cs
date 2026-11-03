using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{
    [Header("Bike Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;
    public float pedalRotationSpeed = 300f; 

    private Rigidbody rb;
    private CyclingLegs legs; // Panggil script IK kaki

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Kunci rotasi sepeda agar tidak jatuh
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        // Cari script CyclingLegs yang ada di dalam karakter
        legs = GetComponentInChildren<CyclingLegs>();
    }

    void FixedUpdate()
    {
        float moveInput = 0f;
        float turnInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput = 1f;
            if (Keyboard.current.sKey.isPressed) moveInput = -1f;
            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;
        }

        // Gerakkan sepeda
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

        // Belok
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Kirim kecepatan ke script IK agar pedal berputar
        if (legs != null)
        {
            legs.SetSpeed(moveInput * pedalRotationSpeed);
        }
    }
}
