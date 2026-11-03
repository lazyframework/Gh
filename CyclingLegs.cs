using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Seret dari Hierarchy")]
    public Transform bottomBracket;   // objek "BB"
    public Transform[] crankParts;    // "Cranks" dan "Pedals"
    public Renderer pedalsRenderer;   // objek "Pedals" (untuk hitung ukuran otomatis)

    [Header("Penyesuaian (biarkan 0 = otomatis)")]
    public float crankLength = 0f;    // jarak BB ke pedal
    public float sideOffset = 0f;     // jarak pedal dari tengah sepeda ke samping
    public float startAngle = 0f;     // ganti 180 kalau kaki kiri/kanan tertukar

    private Animator anim;
    private Transform bike;
    private float angle;
    private float speed;
    private float footUpOffset;       // dihitung otomatis dari bounds pedal

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

            // Panjang crank = setengah ukuran pedal sepanjang arah maju
            if (crankLength <= 0f)
                crankLength = Vector3.Dot(Abs(bike.forward), e);

            // Offset ke samping = setengah lebar pedal
            if (sideOffset <= 0f)
                sideOffset = Vector3.Dot(Abs(bike.right), e);

            // Offset ke atas = setengah tinggi pedal (supaya telapak menapak di atasnya)
            // Ini menggantikan footUpOffset manual
            footUpOffset = Vector3.Dot(Abs(bike.up), e);
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
