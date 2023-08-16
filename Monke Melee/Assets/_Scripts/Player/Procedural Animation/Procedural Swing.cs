using UnityEngine;

public class ProceduralSwing : MonoBehaviour
{
    [Header("Targets")]
    public Transform HandTarget;
    public Transform ShoulderTarget;
    public Transform HipTarget;

    [Header("Character")]
    public Transform HandIKR;
    public Transform HandIKL;

    public Transform Hand;
    public Transform Chest;
    public Transform Hip;

    public Vector3 _hipPositionOffset;
    public Vector3 _chestPositionOffset;

    public Vector3 _hipRotationOffset;
    public Vector3 _chestRotationOffset;

    void FixedUpdate()
    {
        FollowPendulum();
    }



    private void SwingAnimation()
    {
        // Switch to Swing IK Constraint.
        // Rotate PlayerTransform so the head is facing the target.

    }

    private void FollowPendulum()
    {
        Hip.position = Vector3.Lerp(Hip.position, HipTarget.position, Time.deltaTime * 20);
        Hip.rotation = Quaternion.Lerp(Hip.rotation, HipTarget.rotation * Quaternion.Euler(_hipRotationOffset), Time.deltaTime * 20);

        // Chest.position = Vector3.Lerp(Chest.position, ShoulderTarget.position + _chestPositionOffset, Time.deltaTime * 10);
        Chest.rotation = Quaternion.Slerp(Chest.rotation, ShoulderTarget.rotation * Quaternion.Euler(_chestRotationOffset), Time.deltaTime * 20);
    }
}
