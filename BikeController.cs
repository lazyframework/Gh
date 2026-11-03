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
    public Transform bbObject;     // Tarik objek BB (poros tengah) ke sini
    public Transform leftLegBone;  // Tarik tulang Left_UpperLeg karakter
    public Transform rightLegBone; // Tarik tulang Right_UpperLeg karakter

    private Rigidbody rb;
    private Animator playerAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Kunci rotasi sepeda agar tidak jatuh
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        // Cari Animator di karakter
        playerAnimator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        float moveInput = 0f;
        float turnInput = 0f;

        // Baca Input
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput = 1f;
            if (Keyboard.current.sKey.isPressed) moveInput = -1f;
            if (Keyboard.current.aKey.isPressed) turnInput = -1f;
            if (Keyboard.current.dKey.isPressed) turnInput = 1f;
        }

        // --- 1. GERAKKAN SEPEDA ---
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

        // --- 2. BELOKKAN SEPEDA ---
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // --- 3. PUTAR PEDAL (Menggunakan poros BB) ---
        if (cranksObject != null && bbObject != null)
        {
            // Memutar pedal berdasarkan titik tengah BB
            cranksObject.RotateAround(bbObject.position, transform.right, moveInput * pedalRotationSpeed * Time.fixedDeltaTime);
        }

        // --- 4. TRIK KAKI MENEKUK ---
        if (playerAnimator != null)
        {
            // Paksa animasi lari mati agar kaki tidak bergerak sendiri
            playerAnimator.SetFloat("Speed", 0);
            
            // Angka -35f membuat kaki lebih lurus ke bawah mendekati pedal
            // Angka 10f di Z untuk merapatkan kaki ke dalam
            if (leftLegBone != null) leftLegBone.localRotation = Quaternion.Euler(-35f, 0f, 10f);
            if (rightLegBone != null) rightLegBone.localRotation = Quaternion.Euler(-35f, 0f, -10f);
        }
    }
}
