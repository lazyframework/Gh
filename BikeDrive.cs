using UnityEngine;

public class BikeDrive : MonoBehaviour
{
    public Rigidbody rb;
    public Transform bb;                 // objek "BB"
    public Transform cranks, pedals;     // objek "Cranks" dan "Pedals"
    public Transform frontHub, rearHub;  // Front_hub dan Rear_Hub
    public Transform[] frontParts;       // Front_Rim, Front_Tire, Front_Spokes, Front_hub, Front_Valve
    public Transform[] rearParts;        // Rear_Rim, Rear_Tire, Rear_Spokes, Rear_Hub, dst

    public float wheelRadius = 0.25f;
    public float gearRatio = 2.77f;
    public float crankLength = 0.19f;    // jarak as crank ke pedal
    public float pedalSide = 0.12f;      // jarak pedal ke samping dari tengah
    public float angleOffset = 0f;       // tuning supaya posisi awal pedal cocok

    float angle;

    void Update()
    {
        float v = Vector3.Dot(rb.linearVelocity, transform.forward);
        float wheelDeg = v * Time.deltaTime / wheelRadius * Mathf.Rad2Deg;
        Vector3 axis = transform.right;

        foreach (var p in frontParts) p.RotateAround(frontHub.position, axis, wheelDeg);
        foreach (var p in rearParts)  p.RotateAround(rearHub.position,  axis, wheelDeg);

        float crankDeg = wheelDeg / gearRatio;
        if (v < 0) crankDeg = 0f;
        angle += crankDeg;

        cranks.RotateAround(bb.position, axis, crankDeg);
        pedals.RotateAround(bb.position, axis, crankDeg);
    }

    // Posisi telapak kaki di pedal. side = -1 kiri, +1 kanan
    public Vector3 FootPos(int side)
    {
        float a = (angle + angleOffset + (side > 0 ? 180f : 0f)) * Mathf.Deg2Rad;
        Vector3 circle = transform.up * Mathf.Cos(a) + transform.forward * Mathf.Sin(a);
        return bb.position + circle * crankLength + transform.right * (side * pedalSide);
    }

    public Vector3 KneePos(int side)
    {
        return FootPos(side) + transform.forward * 0.4f + transform.up * 0.3f
               + transform.right * (side * 0.1f);
    }

    void OnDrawGizmos()
    {
        if (bb == null) return;
        Gizmos.color = Color.red;   Gizmos.DrawSphere(FootPos(-1), 0.03f);
        Gizmos.color = Color.green; Gizmos.DrawSphere(FootPos(1), 0.03f);
    }
}
