using UnityEngine;

public class BikeController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;
    public float maxSpeed = 15f;

    [Header("Pedals")]
    public Transform crank;                  // Drag objek Cranks ke sini
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
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

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
