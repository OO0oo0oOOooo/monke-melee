using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GibbonSwing : MonoBehaviour
{
    private GibbonRefrences _ref;

    [Header("Swing")]
    private SpringJoint _jointL;
    private SpringJoint _jointR;

    [SerializeField] private LayerMask _swingLayerMask;
    [SerializeField] private Vector3 _swingPivot;

    private bool _swingingL;
    private bool _swingingR;
    public bool SwingingL { get { return _swingingL; } }
    public bool SwingingR { get { return _swingingR; } }

    [SerializeField] private Vector3 _achorOffsetL = new Vector3(-0.1f, 1.1f, 0);
    [SerializeField] private Vector3 _achorOffsetR = new Vector3(0.1f, 1.1f, 0);

    [SerializeField] private float _maxRaycastDistance = 5;
    [SerializeField] private float _minSwingDistance = 0;
    [SerializeField] private float _maxSwingDistance;
    [SerializeField] private float _spring = 4.5f;
    [SerializeField] private float _damper = 7;
    [SerializeField] private float _massScale = 4.5f;

    [Header("IK")]
    public TwoBoneIKConstraint LSillyArm;
    public TwoBoneIKConstraint RSillyArm;

    public TwoBoneIKConstraint LSwingArm;
    public TwoBoneIKConstraint RSwingArm;

    public Transform LSwingTarget;
    public Transform RSwingTarget;

    [SerializeField] private Vector3 _offsetRotationL = Vector3.zero;
    [SerializeField] private Vector3 _offsetRotationR = Vector3.zero;

    private void Awake()
    {
        _ref = GetComponentInParent<GibbonRefrences>();
    }

    private void Update()
    {
        if( Input.GetMouseButton(0) )
            StartSwing(0);

        if( Input.GetMouseButton(1) )
            StartSwing(1);

        if( Input.GetMouseButtonUp(0) )
            EndSwing(0);

        if( Input.GetMouseButtonUp(1) )
            EndSwing(1);

        UpdateIKTargets();
    }

    private void StartSwing(int armIndex)
    {
        if(_swingingL && armIndex == 0)
            return;

        if(_swingingR && armIndex == 1)
            return;

        // if(Physics.SphereCast(_ref.PlayerCamera.transform.position, 0.25f, _ref.PlayerCamera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
        if(Physics.Raycast(_ref.Camera.transform.position, _ref.Camera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
        {
            _swingPivot = hit.point;
            // float distance;

            if(armIndex == 0)
            {
                // _maxSwingDistance = Vector3.Distance(hit.point, _ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetL));

                LSwingTarget.position = hit.point;
                LSillyArm.weight = 0;
                LSwingArm.weight = 1;
            }

            if(armIndex == 1)
            {
                // _maxSwingDistance = Vector3.Distance(hit.point, _ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetR));

                RSillyArm.weight = 0;
                RSwingArm.weight = 1;
            }
            SetupJoint(armIndex);

        }
    }

    private void EndSwing(int armIndex)
    {
        if(armIndex == 0)
        {
            _swingingL = false;
            LSillyArm.weight = 1;
            LSwingArm.weight = 0;

            if(_jointL != null)
                Destroy(_jointL);
        }

        if(armIndex == 1)
        {
            _swingingR = false;
            RSillyArm.weight = 1;
            RSwingArm.weight = 0;

            if(_jointR != null)
                Destroy(_jointR);
        }
    }

    private void SetupJoint(int index)
    {
        if(index == 0)
        {
            if(_jointL != null)
                Destroy(_jointL);

            _swingingL = true;

            _jointL = gameObject.AddComponent<SpringJoint>();
            _jointL.autoConfigureConnectedAnchor = false;
            _jointL.connectedAnchor = _swingPivot;

            _jointL.anchor = _achorOffsetL;

            _jointL.spring = _spring;
            _jointL.damper = _damper;

            _jointL.minDistance = _minSwingDistance;
            _jointL.maxDistance = _maxSwingDistance;

            _jointL.massScale = _massScale;
        }

        if(index == 1)
        {
            if(_jointR != null)
                Destroy(_jointR);

            _swingingR = true;
            
            _jointR = gameObject.AddComponent<SpringJoint>();
            _jointR.autoConfigureConnectedAnchor = false;
            _jointR.connectedAnchor = _swingPivot;

            _jointR.anchor = _achorOffsetR;

            _jointR.spring = _spring;
            _jointR.damper = _damper;

            _jointR.minDistance = _minSwingDistance;
            _jointR.maxDistance = _maxSwingDistance;

            _jointR.massScale = _massScale;
        }
    }

    private void UpdateIKTargets()
    {
        if(_jointL != null)
        {
            LSwingTarget.position = _jointL.connectedAnchor;
            LSwingTarget.rotation = _ref.PlayerTransform.rotation * Quaternion.Euler(_offsetRotationL);
        }

        if(_jointR != null)
        {
            RSwingTarget.position = _jointR.connectedAnchor;
            RSwingTarget.rotation = _ref.PlayerTransform.rotation * Quaternion.Euler(_offsetRotationR);
        }

        // Scroll wheel to change swing distance
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            return;

        Gizmos.color = Color.white;
        // if(Physics.SphereCast(_ref.PlayerCamera.transform.position, 0.25f, _ref.PlayerCamera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
        if(Physics.Raycast(_ref.Camera.transform.position, _ref.Camera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
            Gizmos.DrawSphere(hit.point, 0.1f);

        if(_swingingL)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetL), _jointL.connectedAnchor);
            Gizmos.DrawWireSphere(_jointL.connectedAnchor, 0.1f);
            Gizmos.DrawWireSphere(_ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetL), 0.1f);
        }

        if(_swingingR)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetR), _jointR.connectedAnchor);
            Gizmos.DrawWireSphere(_jointR.connectedAnchor, 0.1f);
            Gizmos.DrawWireSphere(_ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetR), 0.1f);
        }
    }
}
