using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Seret dari Hierarchy")]
    public Transform bottomBracket;   
    public Transform[] crankParts;    
    public Renderer pedalsRenderer;   

    [Header("Penyesuaian")]
    public float crankLength = 0.2f;  // Kita isi manual dulu 0.2 untuk tes
    public float sideOffset = 0.1f;   // Kita isi manual dulu 0.1 untuk tes
    public float footUpOffset = 0.05f;
    public float startAngle = 0f;     

    private Animator anim;
    private Transform bike;
    private float angle;
    private float speed;
    private Vector3 dbgL, dbgR;

    void Awake()
    {
        anim = GetComponent<Animator>();
        var bc = GetComponentInParent<BikeController>();
        bike = bc != null ? bc.transform : transform;
    }

    void Start()
    {
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
        Foot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, HumanBodyBones.RightUpperLeg, angle + startAngle, 1f);
        Foot(AvatarIKGoal.LeftFoot,  AvatarIKHint.LeftKnee,  HumanBodyBones.LeftUpperLeg,  angle + startAngle + 180f, -1f);
    }

    void Foot(AvatarIKGoal goal, AvatarIKHint hint, HumanBodyBones thigh, float a, float side)
    {
        Quaternion spin = Quaternion.AngleAxis(a, bike.right);
        
        // Posisi target
        Vector3 pos = bottomBracket.position
                    + spin * (bike.forward * crankLength)
                    + bike.right * (side * sideOffset)
                    + bike.up * footUpOffset;

        // Simpan untuk bola debug
        if (goal == AvatarIKGoal.RightFoot) dbgR = pos; else dbgL = pos;

        anim.SetIKPositionWeight(goal, 1f);
        anim.SetIKRotationWeight(goal, 1f);
        anim.SetIKPosition(goal, pos);
        anim.SetIKRotation(goal, bike.rotation);

        anim.SetIKHintPositionWeight(hint, 1f);
        anim.SetIKHintPosition(hint, anim.GetBoneTransform(thigh).position + bike.forward * 0.5f);
    }

    // Memunculkan bola debug di Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;   Gizmos.DrawSphere(dbgR, 0.05f); // Kaki Kanan
        Gizmos.color = Color.green; Gizmos.DrawSphere(dbgL, 0.05f); // Kaki Kiri
    }
}
