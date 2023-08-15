using UnityEngine;

public class HandToPointForce : MonoBehaviour
{
    public Rigidbody Hand;
    public Rigidbody ArmL;
    public Transform Target;

    public float Force = 50;

    public float AdditionalHandMass = 0;
    public float AdditionalArmMass = 0;

    void Update()
    {
        Hand.AddForce((Target.position - Hand.position) * Force *  (Hand.mass + AdditionalHandMass));
        ArmL.AddForce(-(Target.position - ArmL.position) * Force *  (ArmL.mass + AdditionalArmMass));
    }

}
