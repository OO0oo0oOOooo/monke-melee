using UnityEngine;

public class SillyArmForce : MonoBehaviour
{
    private GibbonRefrences _ref;

    [SerializeField] private Rigidbody _armL;
    [SerializeField] private Rigidbody _armR;

    [SerializeField] private Rigidbody _legL;
    [SerializeField] private Rigidbody _legR;

    [Header("Arm")]
    public float ArmUpForce = 1;
    public float ArmForwardForce = 1;
    public float ArmAwayForce = 1;
    public float ArmAwayForce2 = 1;

    [Header("Leg")]
    public float LegUpForce = -1;
    public float LegForwardForce = 0;
    public float LegAwayForce = 0;
    public float LegAwayForce2 = 0;

    private void Awake()
    {
        _ref = GetComponentInParent<GibbonRefrences>();
    }

    void FixedUpdate()
    {
        _armL.AddForce(Vector3.up * ArmUpForce * _armL.mass);
        _armL.AddForce(transform.forward * ArmForwardForce * _armL.mass);
        _armL.AddForce((_armL.position - _armR.position) * ArmAwayForce * _armL.mass);

        _armR.AddForce(Vector3.up * ArmUpForce * _armR.mass);
        _armR.AddForce(transform.forward * ArmForwardForce * _armR.mass);
        _armR.AddForce((_armR.position - _armL.position) * ArmAwayForce * _armR.mass);

        if(_ref.GibbonSwing.JointL != null)
        {
            _armR.AddForce((_armR.position - _ref.GibbonSwing.JointL.connectedAnchor) * ArmAwayForce2 * _armR.mass);
        }

        if(_ref.GibbonSwing.JointR != null)
        {
            _armL.AddForce((_armL.position - _ref.GibbonSwing.JointR.connectedAnchor) * ArmAwayForce2 * _armL.mass);
        }


        _legL.AddForce(Vector3.up * LegUpForce * _legL.mass);
        _legL.AddForce(transform.forward * LegForwardForce * _legL.mass);
        _legL.AddForce((_legL.position - _legR.position) * LegAwayForce * _legL.mass);

        _legR.AddForce(Vector3.up * LegUpForce * _legR.mass);
        _legR.AddForce(transform.forward * LegForwardForce * _legR.mass);
        _legR.AddForce((_legR.position - _legL.position) * LegAwayForce * _legR.mass);

        if(_ref.GibbonSwing.JointL != null)
        {
            _legL.AddForce((_legL.position - _ref.GibbonSwing.JointL.connectedAnchor) * LegAwayForce2 * _legL.mass);
            _legR.AddForce((_legR.position - _ref.GibbonSwing.JointL.connectedAnchor) * LegAwayForce2 * _legR.mass);
        }

        if(_ref.GibbonSwing.JointR != null)
        {
            _legL.AddForce((_legL.position - _ref.GibbonSwing.JointR.connectedAnchor) * LegAwayForce2 * _legL.mass);
            _legR.AddForce((_legR.position - _ref.GibbonSwing.JointR.connectedAnchor) * LegAwayForce2 * _legR.mass);
        }
    }
}
