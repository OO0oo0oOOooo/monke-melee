using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class GibbonSwing : MonoBehaviour
{
    private GibbonRefrences _ref;

    private bool _swingingL;
    private bool _swingingR;
    public bool SwingingL { get { return _swingingL; } }
    public bool SwingingR { get { return _swingingR; } }

    [Header("Joint Parameters")]
    private SpringJoint _jointL;
    private SpringJoint _jointR;
    public SpringJoint JointL { get { return _jointL; } }
    public SpringJoint JointR { get { return _jointR; } }

    [SerializeField] private Vector3 _achorOffsetL = new Vector3(-0.1f, 1.1f, 0);
    [SerializeField] private Vector3 _achorOffsetR = new Vector3(0.1f, 1.1f, 0);

    [SerializeField] private float _maxRaycastDistance = 5;
    [SerializeField] private float _minSwingDistance = 0;
    [SerializeField] private float _maxSwingDistance;
    [SerializeField] private float _spring = 4.5f;
    [SerializeField] private float _damper = 7;
    [SerializeField] private float _massScale = 4.5f;

    [Header("Swing IK")]
    [SerializeField] private Transform _swingTargetL;
    [SerializeField] private Transform _swingTargetR;

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

        if(Physics.Raycast(_ref.Camera.transform.position, _ref.Camera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _ref.SimpleCollider.LayerMask))
        {
            // _maxSwingDistance = Vector3.Distance(hit.point, _ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetL));
            // _maxSwingDistance = Vector3.Distance(hit.point, _ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetR));

            _ref.ProceduralAnimation.EnableSwingWeights(armIndex);
            
            SetupJoint(armIndex, hit.point);
            PlaySound(hit.point);
        }

    }

    private void EndSwing(int armIndex)
    {
        _ref.ProceduralAnimation.DisableSwingWeights(armIndex);

        if(armIndex == 0)
        {
            _swingingL = false;

            if(_jointL != null)
                Destroy(_jointL);
        }

        if(armIndex == 1)
        {
            _swingingR = false;

            if(_jointR != null)
                Destroy(_jointR);
        }
    }

    private void SetupJoint(int index, Vector3 pivot)
    {
        if(index == 0)
        {
            if(_jointL != null)
                Destroy(_jointL);

            _swingingL = true;

            _jointL = gameObject.AddComponent<SpringJoint>();
            _jointL.autoConfigureConnectedAnchor = false;
            _jointL.connectedAnchor = pivot;

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
            _jointR.connectedAnchor = pivot;

            _jointR.anchor = _achorOffsetR;

            _jointR.spring = _spring;
            _jointR.damper = _damper;

            _jointR.minDistance = _minSwingDistance;
            _jointR.maxDistance = _maxSwingDistance;

            _jointR.massScale = _massScale;
        }
    }

    // This should be in procedural animation
    private void UpdateIKTargets()
    {
        if(_jointL != null)
        {
            _swingTargetL.position = _jointL.connectedAnchor;
            _swingTargetL.rotation = _ref.PlayerTransform.rotation * Quaternion.Euler(_offsetRotationL);
        }

        if(_jointR != null)
        {
            _swingTargetR.position = _jointR.connectedAnchor;
            _swingTargetR.rotation = _ref.PlayerTransform.rotation * Quaternion.Euler(_offsetRotationR);
        }
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            return;

        Gizmos.color = Color.white;
        if(Physics.Raycast(_ref.Camera.transform.position, _ref.Camera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _ref.SimpleCollider.LayerMask))
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

    private void PlaySound(Vector3 pos)
    {
        AudioSystem.Instance.PlayRandomClipAtPoint((int)GameAudioEnums.Step, pos, 5);
    }
}
