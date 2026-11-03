using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class BikeController : MonoBehaviour
{
    [Header("Bike Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;
    public float pedalRotationSpeed = 300f;

    private Rigidbody rb;
    private CyclingLegs legs;
    private StarterAssetsInputs starterInputs;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        legs = GetComponentInChildren<CyclingLegs>();
        starterInputs = GetComponentInChildren<StarterAssetsInputs>();
    }

    void FixedUpdate()
    {
        float moveInput = 0f;
        float turnInput = 0f;

        if (starterInputs != null)
        {
            moveInput = starterInputs.move.y;
            turnInput = starterInputs.move.x;
        }

        if (Keyboard.current != null && Mathf.Approximately(moveInput, 0f) && Mathf.Approximately(turnInput, 0f))
        {
            if (Keyboard.current.wKey.isPressed) moveInput = 1f;
            if (Keyboard.current.sKey.isPressed) moveInput = -1f;
            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;
        }

        moveInput = Mathf.Clamp(moveInput, -1f, 1f);
        turnInput = Mathf.Clamp(turnInput, -1f, 1f);

        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        if (legs != null)
            legs.SetSpeed(moveInput * pedalRotationSpeed);
    }
}
