using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Seret dari Hierarchy")]
    public Transform bottomBracket;   // objek "BB"
    public Transform[] crankParts;    // objek "Cranks" dan "Pedals"

    [Header("Penyetelan (lihat bola merah/hijau di Scene view)")]
    public float crankLength = 0.25f;   // jarak BB ke pedal
    public float sideOffset = 0.15f;    // jarak pedal ke samping dari tengah sepeda
    public float footUpOffset = 0.05f;  // angkat kaki dari titik pedal
    public float startAngle = 0f;       // sudut awal (0 atau 180)
    public float kneeForward = 0.5f;    // arah tekuk lutut ke depan

    private Animator anim;
    private Transform bike;
    private float angle;
    private float speed;
    private HashSet<string> paramNames = new HashSet<string>();

    void Awake()
    {
        anim = GetComponent<Animator>();
        var bc = GetComponentInParent<BikeController>();
        bike = bc != null ? bc.transform : transform;
        foreach (var p in anim.parameters) paramNames.Add(p.name);
    }

    public void SetSpeed(float s) { speed = s; }

    void Update()
    {
        // ThirdPersonController dimatikan, jadi parameter Animator diatur di sini
        SetBool("Grounded", true);
        SetBool("FreeFall", false);
        SetBool("Jump", false);
        SetFloat("Speed", 0f);
        SetFloat("MotionSpeed", 1f);

        float delta = speed * Time.deltaTime;
        angle += delta;
        foreach (var part in crankParts)
            if (part != null)
                part.RotateAround(bottomBracket.position, bike.right, delta);
    }

    // Posisi target kaki. side: +1 kanan, -1 kiri
    Vector3 FootPos(Transform b, float a, float side)
    {
        Quaternion spin = Quaternion.AngleAxis(a, b.right);
        return bottomBracket.position
             + spin * (b.forward * crankLength)
             + b.right * (side * sideOffset)
             + b.up * footUpOffset;
    }

    void OnAnimatorIK(int layerIndex)
    {
        Apply(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, HumanBodyBones.RightUpperLeg,
              FootPos(bike, angle + startAngle, 1f));
        Apply(AvatarIKGoal.LeftFoot, AvatarIKHint.LeftKnee, HumanBodyBones.LeftUpperLeg,
              FootPos(bike, angle + startAngle + 180f, -1f));
    }

    void Apply(AvatarIKGoal goal, AvatarIKHint hint, HumanBodyBones thigh, Vector3 pos)
    {
        anim.SetIKPositionWeight(goal, 1f);
        anim.SetIKRotationWeight(goal, 1f);
        anim.SetIKPosition(goal, pos);
        anim.SetIKRotation(goal, bike.rotation);

        Transform t = anim.GetBoneTransform(thigh);
        if (t != null)
        {
            anim.SetIKHintPositionWeight(hint, 1f);
            anim.SetIKHintPosition(hint, t.position + bike.forward * kneeForward);
        }
    }

    // Bola penanda: terlihat di Scene view, juga sebelum Play
    void OnDrawGizmos()
    {
        if (bottomBracket == null) return;
        var bc = GetComponentInParent<BikeController>();
        Transform b = bc != null ? bc.transform : transform;

        float a = Application.isPlaying ? angle + startAngle : startAngle;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(FootPos(b, a, 1f), 0.05f);     // kaki kanan
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(FootPos(b, a + 180f, -1f), 0.05f); // kaki kiri
    }

    void SetBool(string n, bool v)   { if (paramNames.Contains(n)) anim.SetBool(n, v); }
    void SetFloat(string n, float v) { if (paramNames.Contains(n)) anim.SetFloat(n, v); }
}
