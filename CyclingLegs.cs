
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CyclingLegs : MonoBehaviour
{
    [Header("Referensi Sepeda")]
    public Transform bottomBracket;
    public Transform[] crankParts;
    public Renderer pedalsRenderer;

    [Header("Pengaturan Pedal")]
    [Min(0.01f)] public float crankLength = 0.3f;
    public float sideOffset = 0.1f;
    public float footUpOffset = 0.05f;
    public float startAngle = 0f;

    [Header("IK Kaki")]
    [Range(0f, 1f)] public float ikWeight = 1f;
    public float kneeForwardOffset = 0.3f;

    private Animator anim;
    private Transform bike;
    private float angle;
    private float speed;
    private Quaternion[] initialCrankRotations;

    void Awake()
    {
        anim = GetComponent<Animator>();

        var bc = GetComponentInParent<BikeController>();
        bike = bc != null ? bc.transform : transform;

        if (crankParts != null)
        {
            initialCrankRotations = new Quaternion[crankParts.Length];

            for (int i = 0; i < crankParts.Length; i++)
            {
                if (crankParts[i] != null)
                    initialCrankRotations[i] = crankParts[i].rotation;
            }
        }
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }

    void Update()
    {
        if (bottomBracket == null || bike == null)
            return;

        angle += speed * Time.deltaTime;

        // Rotasi crank pada sumbu samping sepeda.
        // Jika arah putaran salah, balik tanda speed.
        if (crankParts != null)
        {
            foreach (Transform part in crankParts)
            {
                if (part != null)
                {
                    // Catatan: semua objek yang dimasukkan di sini
                    // harus memang bagian crank yang ingin diputar.
                    part.RotateAround(
                        bottomBracket.position,
                        bike.right,
                        speed * Time.deltaTime
                    );
                }
            }
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (bottomBracket == null || bike == null || anim == null)
            return;

        SetFoot(
            AvatarIKGoal.RightFoot,
            AvatarIKHint.RightKnee,
            HumanBodyBones.RightUpperLeg,
            angle + startAngle,
            1f
        );

        SetFoot(
            AvatarIKGoal.LeftFoot,
            AvatarIKHint.LeftKnee,
            HumanBodyBones.LeftUpperLeg,
            angle + startAngle + 180f,
            -1f
        );
    }

    void SetFoot(
        AvatarIKGoal goal,
        AvatarIKHint hint,
        HumanBodyBones thigh,
        float degrees,
        float side
    )
    {
        Quaternion orbit = Quaternion.AngleAxis(degrees, bike.right);

        // Titik pedal bergerak pada lingkaran vertikal.
        Vector3 pedalPosition =
            bottomBracket.position
            + orbit * (bike.forward * crankLength)
            + bike.right * (side * sideOffset)
            + bike.up * footUpOffset;

        anim.SetIKPositionWeight(goal, ikWeight);
        anim.SetIKPosition(goal, pedalPosition);

        // Orientasi dasar telapak kaki.
        anim.SetIKRotationWeight(goal, ikWeight);
        anim.SetIKRotation(goal, bike.rotation);

        Transform thighBone = anim.GetBoneTransform(thigh);

        if (thighBone != null)
        {
            anim.SetIKHintPositionWeight(hint, ikWeight);
            anim.SetIKHintPosition(
                hint,
                thighBone.position
                + bike.forward * kneeForwardOffset
            );
        }
    }
}
