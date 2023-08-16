using UnityEngine;

public class CopyLimbPostition : MonoBehaviour
{
    [Header("Hand")]
    [SerializeField] private Transform _handL;
    [SerializeField] private Transform _handR;

    [SerializeField] private Transform _handIKTargetL;
    [SerializeField] private Transform _handIKTargetR;

    public Vector3 OffsetPositionHandL = Vector3.zero;
    public Vector3 OffsetPositionHandR = Vector3.zero;

    public Vector3 OffsetRotationHandL = Vector3.zero;
    public Vector3 OffsetRotationHandR = Vector3.zero;

    [Header("Foot")]
    [SerializeField] private Transform _footL;
    [SerializeField] private Transform _footR;

    [SerializeField] private Transform _footIKTargetL;
    [SerializeField] private Transform _footIKTargetR;

    public Vector3 OffsetPositionFootL = Vector3.zero;
    public Vector3 OffsetPositionFootR = Vector3.zero;

    public Vector3 OffsetRotationFootL = Vector3.zero;
    public Vector3 OffsetRotationFootR = Vector3.zero;

    void Update()
    {
        Hand();
        Foot();
    }

    private void Hand()
    {
        // Set Offsets
        _handL.localPosition = OffsetPositionHandL;
        _handR.localPosition = OffsetPositionHandR;

        _handL.localRotation = Quaternion.Euler(OffsetRotationHandL);
        _handR.localRotation = Quaternion.Euler(OffsetRotationHandR);

        // IK copy target
        _handIKTargetL.localPosition = transform.InverseTransformPoint(_handL.position);
        _handIKTargetR.localPosition = transform.InverseTransformPoint(_handR.position);

        _handIKTargetL.rotation = _handL.rotation;
        _handIKTargetR.rotation = _handR.rotation;
    }

    private void Foot()
    {
        // Set Offsets
        _footL.localPosition = OffsetPositionFootL;
        _footR.localPosition = OffsetPositionFootR;

        _footL.localRotation = Quaternion.Euler(OffsetRotationFootL);
        _footR.localRotation = Quaternion.Euler(OffsetRotationFootR);

        // IK copy target
        _footIKTargetL.localPosition = transform.InverseTransformPoint(_footL.position);
        _footIKTargetR.localPosition = transform.InverseTransformPoint(_footR.position);

        _footIKTargetL.rotation = _footL.rotation;
        _footIKTargetR.rotation = _footR.rotation;
    }
}
