using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float kecepatanMaju = 10f;
    public float kecepatanBelok = 80f;

    [Header("Pengaturan Pedal")]
    public Transform crank; 
    public float kecepatanPutarPedal = 150f;

    private Rigidbody rb;

    void Start()
    {
        // Ambil komponen Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Jika tidak ada Rigidbody, tambahkan otomatis agar tidak error
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 50f;
        }
    }

    void Update()
    {
        float maju = 0f;
        float belok = 0f;

        // --- CEK APAKAH KEYBOARD TERHUBUNG ---
        if (Keyboard.current != null)
        {
            // Cek tombol W dan S
            if (Keyboard.current.wKey.isPressed) maju = 1f;
            if (Keyboard.current.sKey.isPressed) maju = -1f;
            
            // Cek tombol A dan D
            if (Keyboard.current.dKey.isPressed) belok = 1f;
            if (Keyboard.current.aKey.isPressed) belok = -1f;
        }
        else
        {
            Debug.LogError("Keyboard tidak terdeteksi oleh Unity Input System!");
        }

        // --- LOGIKA PUTAR PEDAL ---
        if (crank != null && Mathf.Abs(maju) > 0.1f)
        {
            // Memutar pedal
            crank.Rotate(Vector3.right * maju * kecepatanPutarPedal * Time.deltaTime);
            Debug.Log("Pedal berputar!");
        }

        // --- LOGIKA MAJU & BELOK ---
        if (rb != null)
        {
            // Maju / Mundur (pakai kecepatan langsung, bukan AddForce agar lebih responsif)
            Vector3 gerakan = transform.forward * maju * kecepatanMaju;
            rb.linearVelocity = new Vector3(gerakan.x, rb.linearVelocity.y, gerakan.z);

            // Belok
            if (Mathf.Abs(belok) > 0.1f)
            {
                float rotasiY = belok * kecepatanBelok * Time.deltaTime;
                transform.Rotate(0f, rotasiY, 0f);
            }
        }
    }
}
