using UnityEngine;

public class RiderFootIK : MonoBehaviour
{
    [Header("Referensi Utama")]
    public Transform cranks; // Drag objek Cranks ke sini

    [Header("Jarak Kaki (Geser Kiri/Kanan)")]
    public float jarakKakiKiri = -0.2f; // Angka minus = ke kiri
    public float jarakKakiKanan = 0.2f; // Angka positif = ke kanan

    [Header("IK Settings")]
    [Range(0f, 1f)] public float footIKWeight = 1f;
    [Range(0f, 1f)] public float footRotationWeight = 1f;

    [Header("Offset Kaki (Atur Naik/Turun)")]
    public Vector3 leftFootOffset = new Vector3(0f, 0.08f, 0f);
    public Vector3 rightFootOffset = new Vector3(0f, 0.08f, 0f);

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || cranks == null) return;

        // --- KAKI KIRI ---
        Vector3 leftTargetPos = cranks.position + (cranks.right * jarakKakiKiri) + leftFootOffset;
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, footRotationWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftTargetPos);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, cranks.rotation);

        // --- KAKI KANAN ---
        Vector3 rightTargetPos = cranks.position + (cranks.right * jarakKakiKanan) + rightFootOffset;
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, footIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, footRotationWeight);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightTargetPos);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, cranks.rotation);
    }
}
