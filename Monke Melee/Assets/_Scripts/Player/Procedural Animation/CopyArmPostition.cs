using UnityEngine;

public class CopyArmPostition : MonoBehaviour
{
    public Transform ArmL;
    public Transform ArmR;

    public Transform HandIKL;
    public Transform HandIKR;

    public Vector3 OffsetPositionArmL;
    public Vector3 OffsetPositionArmR;

    public Vector3 OffsetRotationArmL;
    public Vector3 OffsetRotationArmR;

    void Update()
    {
        // Convert to local space
        HandIKL.localPosition = transform.InverseTransformPoint(ArmL.position) + (ArmL.localRotation * OffsetPositionArmL);
        HandIKR.localPosition = transform.InverseTransformPoint(ArmR.position) + (ArmR.localRotation * OffsetPositionArmR);

        HandIKL.rotation = ArmL.rotation * Quaternion.Euler(OffsetRotationArmL);
        HandIKR.rotation = ArmR.rotation * Quaternion.Euler(OffsetRotationArmR);
    }
}
