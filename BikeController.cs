using UnityEngine;
using UnityEngine.InputSystem;

public class BikeController : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        float moveZ = 0f;
        float moveX = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveZ = 1f;
            if (Keyboard.current.sKey.isPressed) moveZ = -1f;
            if (Keyboard.current.dKey.isPressed) moveX = 1f;
            if (Keyboard.current.aKey.isPressed) moveX = -1f;
        }

        // Gerakkan objek secara paksa
        transform.Translate(new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime);

        // Print ke Console untuk memastikan tombol kebaca
        if (moveZ != 0 || moveX != 0)
        {
            Debug.Log("Tombol terdeteksi! Bergerak...");
        }
    }
}
