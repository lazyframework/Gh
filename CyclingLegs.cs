using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Seret dari Hierarchy")]
    public Transform bottomBracket;   // objek "BB"
    public Transform[] crankParts;    // objek "Cranks" dan "Pedals"
    public Renderer pedalsRenderer;   // objek "Pedals" (untuk hitung ukuran otomatis)

    [Header("Penyesuaian (biarkan 0 = otomatis)")]
    public float startAngle = 0f;     // ganti 180 kalau kaki kiri/kanan tertukar

    private Animator anim;
    private Transform bike;
    private float angle;
    private float speed;
    
    // Nilai otomatis
    private float crankLength;
    private float sideOffset;
    private float footUpOffset;

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

        if (pedalsRenderer != null)
        {
            Vector3 e = pedalsRenderer.bounds.extents;
            
            // Hitung semua nilai secara otomatis dari ukuran mesh pedal
            crankLength = Vector3.Dot(Abs(bike.forward), e);
            sideOffset = Vector3.Dot(Abs(bike.right), e);
            footUpOffset = Vector3.Dot(Abs(bike.up), e);
        }
        else
        {
            Debug.LogWarning("Pedals Renderer belum dimasukkan!");
        }
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
        Foot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, HumanBodyBones.RightUpperLeg, angle + startAngle, 1f);
        Foot(AvatarIKGoal.LeftFoot,  AvatarIKHint.LeftKnee,  HumanBodyBones.LeftUpperLeg,  angle + startAngle + 180f, -1f);
    }

    void Foot(AvatarIKGoal goal, AvatarIKHint hint, HumanBodyBones thigh, float a, float side)
    {
        Quaternion spin = Quaternion.AngleAxis(a, bike.right);
        
        // Posisi target dihitung otomatis
        Vector3 pos = bottomBracket.position
                    + spin * (bike.forward * crankLength)
                    + bike.right * (side * sideOffset)
                    + bike.up * footUpOffset;

        anim.SetIKPositionWeight(goal, 1f);
        anim.SetIKRotationWeight(goal, 1f);
        anim.SetIKPosition(goal, pos);
        anim.SetIKRotation(goal, bike.rotation);

        anim.SetIKHintPositionWeight(hint, 1f);
        anim.SetIKHintPosition(hint, anim.GetBoneTransform(thigh).position + bike.forward * 0.5f);
    }

    static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
}
