using UnityEngine;

public class PutarPedal : MonoBehaviour
{
    public float kecepatanPutar = 200f;

    void Update()
    {
        // Memaksa pedal berputar terus menerus di sumbu X
        transform.Rotate(Vector3.right * kecepatanPutar * Time.deltaTime);
    }
}
