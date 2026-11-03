using UnityEngine;

public class RiderFootIK : MonoBehaviour
{
    [Header("Referensi")]
    public Transform pedals;           // Drag objek "Pedals" ke sini

    [Header("IK Settings")]
    [Range(0f, 1f)] public float footIKWeight = 1f;
    [Range(0f, 1f)] public float footRotationWeight = 0.8f;

    [Header("Offset Kaki (atur ini)")]
    public Vector3 leftFootOffset = new Vector3(-0.15f, 0.02f, 0f);   // Kiri
    public Vector3 rightFootOffset = new Vector3(0.15f, 0.02f, 0f);  // Kanan

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || pedals == null) return;

        // ===== KAKI KIRI =====
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, footRotationWeight);

        Vector3 leftPos = pedals.TransformPoint(leftFootOffset);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftPos);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, pedals.rotation);

        // ===== KAKI KANAN =====
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, footIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, footRotationWeight);

        Vector3 rightPos = pedals.TransformPoint(rightFootOffset);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightPos);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, pedals.rotation);
    }
}
