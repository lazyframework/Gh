using UnityEngine;

public class RiderFootIK : MonoBehaviour
{
    [Header("Pedal References")]
    public Transform leftPedal;     // Drag pedal kiri ke sini
    public Transform rightPedal;    // Drag pedal kanan ke sini

    [Header("IK Settings")]
    [Range(0f, 1f)] public float footIKWeight = 1f;
    [Range(0f, 1f)] public float footRotationWeight = 1f;

    [Header("Offset Kaki (atur sampai pas)")]
    public Vector3 leftFootOffset = new Vector3(0f, 0.08f, 0f);
    public Vector3 rightFootOffset = new Vector3(0f, 0.08f, 0f);

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // Kaki Kiri
        if (leftPedal != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, footRotationWeight);

            Vector3 targetPos = leftPedal.position + leftPedal.TransformDirection(leftFootOffset);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, targetPos);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftPedal.rotation);
        }

        // Kaki Kanan
        if (rightPedal != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, footIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, footRotationWeight);

            Vector3 targetPos = rightPedal.position + rightPedal.TransformDirection(rightFootOffset);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, targetPos);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightPedal.rotation);
        }
    }
}
