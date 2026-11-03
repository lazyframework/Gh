using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Seret dari Hierarchy")]
    public Transform bottomBracket;      // objek "BB"
    public Transform[] crankParts;       // "Cranks" dan "Pedals"
    public Transform pedalRight;         // objek pedal kanan (anak dari Cranks)
    public Transform pedalLeft;          // objek pedal kiri (anak dari Cranks)

    [Header("Opsional (biarkan 0 = otomatis)")]
    public float startAngle = 0f;        // 180 kalau kaki kiri/kanan tertukar

    private Animator anim;
    private Transform bike;
    private float angle;
    private float speed;

    // Titik target yang dihitung otomatis tiap frame
    private Vector3 targetR, targetL;

    void Awake()
    {
        anim = GetComponent<Animator>();
        var bc = GetComponentInParent<BikeController>();
        bike = bc != null ? bc.transform : transform;
    }

    void Start()
    {
        // Karena ThirdPersonController dimatikan
        anim.SetBool("Grounded", true);
        anim.SetFloat("Speed", 0f);
        anim.SetFloat("MotionSpeed", 1f);
    }

    public void SetSpeed(float s) { speed = s; }

    void Update()
    {
        float delta = speed * Time.deltaTime;
        angle += delta;
        foreach (var part in crankParts)
            part.RotateAround(bottomBracket.position, bike.right, delta);
    }

    void OnAnimatorIK(int layerIndex)
    {
        // Hitung target dari posisi pedal yang sebenarnya (otomatis)
        // Pedal kanan = pedalRight.position, kiri = pedalLeft.position
        // Tambah sedikit ke atas supaya telapak (bukan pergelangan) yang menapak
        targetR = pedalRight != null ? pedalRight.position + bike.up * 0.03f : Vector3.zero;
        targetL = pedalLeft  != null ? pedalLeft.position  + bike.up * 0.03f : Vector3.zero;

        Foot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, HumanBodyBones.RightUpperLeg, targetR);
        Foot(AvatarIKGoal.LeftFoot,  AvatarIKHint.LeftKnee,  HumanBodyBones.LeftUpperLeg,  targetL);
    }

    void Foot(AvatarIKGoal goal, AvatarIKHint hint, HumanBodyBones thigh, Vector3 pos)
    {
        anim.SetIKPositionWeight(goal, 1f);
        anim.SetIKRotationWeight(goal, 1f);
        anim.SetIKPosition(goal, pos);
        anim.SetIKRotation(goal, bike.rotation);

        anim.SetIKHintPositionWeight(hint, 1f);
        anim.SetIKHintPosition(hint, anim.GetBoneTransform(thigh).position + bike.forward * 0.5f);
    }
}
