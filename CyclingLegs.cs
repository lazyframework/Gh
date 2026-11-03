using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Seret dari Hierarchy")]
    public Transform bottomBracket;   
    public Transform[] crankParts;    
    public Renderer pedalsRenderer;   

    [Header("Penyesuaian (Ubah saat Play Mode jika belum pas)")]
    public float crankLength = 0.3f;  
    public float sideOffset = 0.1f;   
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

    public void SetSpeed(float s) { speed = s; }

    void Update()
    {
        float delta = speed * Time.deltaTime;
        angle += delta;
        
        // Memutar objek Cranks/Pedals secara manual
        if (crankParts != null)
        {
            foreach (var part in crankParts)
            {
                if(part != null) part.RotateAround(bottomBracket.position, bike.right, delta);
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        // Menarik kaki kiri dan kanan ke posisi pedal
        Foot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, HumanBodyBones.RightUpperLeg, angle + startAngle, 1f);
        Foot(AvatarIKGoal.LeftFoot,  AvatarIKHint.LeftKnee,  HumanBodyBones.LeftUpperLeg,  angle + startAngle + 180f, -1f);
    }

    void Foot(AvatarIKGoal goal, AvatarIKHint hint, HumanBodyBones thigh, float a, float side)
    {
        // Menghitung posisi pedal
        Quaternion spin = Quaternion.AngleAxis(a, bike.right);
        Vector3 pos = bottomBracket.position
                    + spin * (bike.forward * crankLength)
                    + bike.right * (side * sideOffset)
                    + bike.up * footUpOffset;

        // Menggerakkan kaki
        anim.SetIKPositionWeight(goal, 1f);
        anim.SetIKRotationWeight(goal, 1f);
        anim.SetIKPosition(goal, pos);
        anim.SetIKRotation(goal, bike.rotation);

        // Lutut menekuk ke depan
        Transform thighBone = anim.GetBoneTransform(thigh);
        if (thighBone != null)
        {
            anim.SetIKHintPositionWeight(hint, 1f);
            anim.SetIKHintPosition(hint, thighBone.position + bike.forward * 0.5f);
        }
    }
}
