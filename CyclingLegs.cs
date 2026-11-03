using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Seret dari Hierarchy")]
    public Transform bottomBracket;   // objek "BB"
    public Transform[] crankParts;    // objek "Cranks" dan "Pedals"
    public Renderer pedalsRenderer;   // objek "Pedals" (untuk hitung ukuran otomatis)

    [Header("Penyesuaian (biarkan 0 = otomatis)")]
    public float crankLength = 0f;    
    public float sideOffset = 0f;     
    public float footUpOffset = 0.05f;
    public float startAngle = 0f;     

    private Animator anim;
    private Transform bike;
    private float angle;
    private float speed;

    void Awake()
    {
        anim = GetComponent<Animator>();
        var bc = GetComponentInParent<BikeController>();
        bike = bc != null ? bc.transform : transform;
    }

    void Start()
    {
        // Memastikan animator tidak mengunci kaki di pose lari/jatuh
        anim.SetBool("Grounded", true);
        anim.SetFloat("Speed", 0f);
        anim.SetFloat("MotionSpeed", 1f);

        // Perhitungan otomatis ukuran pedal
        if (pedalsRenderer != null)
        {
            Vector3 e = pedalsRenderer.bounds.extents;
            if (crankLength <= 0f) crankLength = Vector3.Dot(Abs(bike.forward), e);
            if (sideOffset <= 0f)  sideOffset  = Vector3.Dot(Abs(bike.right), e);
        }
    }

    public void SetSpeed(float s) { speed = s; }

    void Update()
    {
        float delta = speed * Time.deltaTime;
        angle += delta;
        
        // Memutar objek pedal secara visual
        foreach (var part in crankParts)
        {
            if (part != null)
                part.RotateAround(bottomBracket.position, bike.right, delta);
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        // Kaki Kanan
        Foot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, HumanBodyBones.RightUpperLeg, angle + startAngle, 1f);
        // Kaki Kiri (ditambah 180 derajat agar bergantian)
        Foot(AvatarIKGoal.LeftFoot,  AvatarIKHint.LeftKnee,  HumanBodyBones.LeftUpperLeg,  angle + startAngle + 180f, -1f);
    }

    void Foot(AvatarIKGoal goal, AvatarIKHint hint, HumanBodyBones thigh, float a, float side)
    {
        Quaternion spin = Quaternion.AngleAxis(a, bike.right);
        
        // Menghitung posisi target (pedal)
        Vector3 pos = bottomBracket.position
                    + spin * (bike.forward * crankLength)
                    + bike.right * (side * sideOffset)
                    + bike.up * footUpOffset;

        // Set Posisi Kaki
        anim.SetIKPositionWeight(goal, 1f);
        anim.SetIKPosition(goal, pos);

        // Set Rotasi Kaki agar TIDAK PATAH/JUNGKIR BALIK
        anim.SetIKRotationWeight(goal, 1f);
        anim.SetIKRotation(goal, Quaternion.identity); 

        // Set Arah Lutut (Hint)
        anim.SetIKHintPositionWeight(hint, 1f);
        Transform thighBone = anim.GetBoneTransform(thigh);
        if (thighBone != null)
        {
            anim.SetIKHintPosition(hint, thighBone.position + bike.forward * 0.5f);
        }
    }

    static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
}
