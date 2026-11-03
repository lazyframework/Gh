using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{
    [Header("Bike Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 80f;
    public float pedalRotationSpeed = 300f; 

    [Header("Referensi")]
    public Transform cranksObject; // Tarik objek Cranks ke sini
    public Transform leftLegBone;  // Tarik tulang paha kiri karakter (LeftUpLeg)
    public Transform rightLegBone; // Tarik tulang paha kanan karakter (RightUpLeg)

    private Rigidbody rb;
    private Animator playerAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        // Cari Animator di karakter
        playerAnimator = GetComponentInChildren<Animator>();
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

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // --- MEMUTAR PEDAL ---
        if (cranksObject != null)
        {
            cranksObject.Rotate(Vector3.right * moveInput * pedalRotationSpeed * Time.fixedDeltaTime);
        }

        // --- TRIK KAKI MENEKUK ---
        if (moveInput == 0 && playerAnimator != null)
        {
            // Jika sepeda diam, paksa animasi jalan mati
            playerAnimator.SetFloat("Speed", 0);
            
            // Paksa tulang paha menekuk ke depan
            if (leftLegBone != null) leftLegBone.localRotation = Quaternion.Euler(-60f, 0f, 0f);
            if (rightLegBone != null) rightLegBone.localRotation = Quaternion.Euler(-60f, 0f, 0f);
        }
    }
}
