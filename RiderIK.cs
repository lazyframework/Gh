using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RiderIK : MonoBehaviour
{
    public BikeDrive bike;
    public Transform grips;          // objek "Grips"
    public float handWidth = 0.28f;  // jarak tangan dari tengah stang
    [Range(0, 1)] public float weight = 1f;

    Animator anim;
    void Awake() { anim = GetComponent<Animator>(); }

    void OnAnimatorIK(int layer)
    {
        Foot(AvatarIKGoal.LeftFoot,  AvatarIKHint.LeftKnee,  -1);
        Foot(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee, +1);

        if (grips != null)
        {
            Hand(AvatarIKGoal.LeftHand,  grips.position - bike.transform.right * handWidth);
            Hand(AvatarIKGoal.RightHand, grips.position + bike.transform.right * handWidth);
        }
    }

    void Foot(AvatarIKGoal g, AvatarIKHint h, int side)
    {
        anim.SetIKPositionWeight(g, weight);
        anim.SetIKRotationWeight(g, weight);
        anim.SetIKPosition(g, bike.FootPos(side));
        anim.SetIKRotation(g, bike.transform.rotation);   // telapak datar mengikuti sepeda
        anim.SetIKHintPositionWeight(h, weight);
        anim.SetIKHintPosition(h, bike.KneePos(side));
    }

    void Hand(AvatarIKGoal g, Vector3 pos)
    {
        anim.SetIKPositionWeight(g, weight);
        anim.SetIKPosition(g, pos);
    }
}
