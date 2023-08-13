using UnityEngine;

public class SwingTest : MonoBehaviour
{
    [Header("Targets")]
    public Transform HandTarget;
    public Transform ShoulderTarget;
    public Transform HipTarget;

    [Header("Character")]
    public Rigidbody HandR;
    public Rigidbody ArmL;

    public Transform Hand;
    public Transform Chest;
    public Transform Hip;

    public float ForceHand = 5;
    public float ForceArm = 5;
    public float AdditionalHandMass = 0;
    public float AdditionalArmMass = 0;

    void FixedUpdate()
    {
        HandR.AddForce((HandTarget.position - HandR.position) * ForceHand *  (HandR.mass + AdditionalHandMass));
        ArmL.AddForce(-(HandTarget.position - ArmL.position) * ForceHand *  (ArmL.mass + AdditionalArmMass));

        Chest.position = Vector3.Lerp(Chest.position, ShoulderTarget.position, Time.deltaTime * 10);
        Chest.rotation = Quaternion.Lerp(Chest.rotation, ShoulderTarget.rotation, Time.deltaTime * 10);

        Hip.position = Vector3.Lerp(Hip.position, HipTarget.position, Time.deltaTime * 10);
        Hip.rotation = Quaternion.Lerp(Hip.rotation, HipTarget.rotation, Time.deltaTime * 10);
    }

}
