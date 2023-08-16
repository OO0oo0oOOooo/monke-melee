using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Rigidbody))]
public partial class GibbonMovement : MonoBehaviour
{
    #region Unity Functions
    private void Awake()
    {
        _ref = GetComponent<GibbonRefrences>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
            _fly = !_fly;

        if( Input.GetMouseButton(0) )
            StartSwing(0);

        if( Input.GetMouseButton(1) )
            StartSwing(1);

        if( Input.GetMouseButtonUp(0) )
            EndSwing(0);

        if( Input.GetMouseButtonUp(1) )
            EndSwing(1);

        _inputDir = _ref.CustomInput.InputDirPlayer;

        // Reset able to climb after landing
        // if(!_ableToClimb && _player.PlayerCollider.AdverageDistance < thresholdBot)
            // _ableToClimb = true;

        _state =  GetMovementState();

        switch (_state)
        {
            // case MovementState.Ground:
            //     UpdateGround();
            //     break;
            // case MovementState.Air:
            //     UpdateAir();
            //     break;
            case MovementState.Swing:
                UpdateSwing();
                break;
            default:
                break;
        }
    }
    
    private void FixedUpdate()
    {
        // Sync before changing
        _vel = _ref.Rigidbody.velocity;

        if (JumpPending && Grounded)
            Jump();
            // Jump(_player.PlayerCamera.transform.forward, 2);

        switch (GetMovementState())
        {
            case MovementState.Ground:
                FixedGround();
                break;
            case MovementState.Air:
                FixedAir();
                break;
            case MovementState.Swing:
                FixedSwing();
                break;
            case MovementState.Fly:
                FixedFly();
                break;
            default:
                break;
        }
        
        _ref.Rigidbody.velocity = _vel;

        _ref.SimpleCollider.IsGrounded = false;
        _ref.SimpleCollider.ContactNormal = Vector3.zero;
    }
    #endregion

    #region Movement Functions
    private MovementState GetMovementState()
    {
        if(_flyToggle && _fly)
            return MovementState.Fly;

        if(_swingingL || _swingingR && !Grounded)
            return MovementState.Swing;

        if(Grounded) // && _ableToClimb && _ableToJump)
            return MovementState.Ground;
        else
            return MovementState.Air;
    }
    
    private void UpdateGround()
    {
        // Vector3 right = Vector3.Cross(Normal, _player.PlayerTransform.forward);
        // Vector3 forward = Vector3.Cross(right, Normal);

        // TODO: This should be done in Procedural Animation
        // Rotate Player Transform to align with surface
        // if(Normal != Vector3.zero)
            // _player.PlayerTransform.rotation = Quaternion.Lerp(_player.PlayerTransform.rotation, Quaternion.LookRotation(forward, Normal), Time.deltaTime * 20f);

        // TODO: Try using the the same method i used in Procedural Walk IK (Distance to surface)
        // Set player height above ground
        // if(Grounded && _player.PlayerCollider.AdverageDistance < thresholdBot)
            // _player.PlayerTransform.position += Normal * 0.01f;

        // if(Grounded && _player.PlayerCollider.AdverageDistance > thresholdTop)
            // _player.PlayerTransform.position -= Normal * 0.01f;
    }

    private void UpdateAir()
    {
        
    }

    private void UpdateSwing()
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

    private void FixedGround()
    {
        // Grounded Branch Detection

        _inputDir = Vector3.Cross(Vector3.Cross(Normal, _inputDir), Normal).normalized;

        // _vel = _inputDir * 10f;
        GroundAccelerate();
        ApplyFriction(_friction);
    }
    
    private void FixedAir()
    {
        // Air Ground/Branch Detection
        // Swing Point Detection
        // AirAccelerate();
        SwingAccelerate();
        ApplyGravity();
    }
    
    private void FixedSwing()
    {
        SwingAccelerate();
        ApplyGravity();
    }

    private void FixedFly()
    {
        float y;
        if(JumpPending)
            y = 1;
        else if(_ref.CustomInput.DuckingPending)
            y = -1;
        else
            y = 0;

        // _vel += ((_cam.transform.forward * CustomInput.Instance.InputRaw.y) + (Vector3.up * y) + (_cam.transform.right * CustomInput.Instance.InputRaw.x)) * 10f;
        Vector3 flyDir = _ref.PlayerCamera.transform.rotation * new Vector3(_ref.CustomInput.InputAxisRaw.x, y, _ref.CustomInput.InputAxisRaw.z);

        _vel += flyDir * 10f;
        ApplyFriction(_friction);
    }
    #endregion

    #region Acceleration
    private void GroundAccelerate()
    {
        float speedMag = Vector3.Dot(_vel, _inputDir);
        Accelerate(_inputDir, speedMag, _groundBaseLimit, _groundAcceleration);

        if (_clampGroundSpeed)
            ClampVel(_groundBaseLimit);
    }

    private void AirAccelerate()
    {
        Vector3 hVel = _vel;
        hVel.y = 0;

        float speedMag = Vector3.Dot(hVel, _inputDir);
        Accelerate(_inputDir, speedMag, _airBaseLimit, _airAcceleration);
    }

    private void AirAccelerate2()
    {
        Vector3 hVel = _vel;
        hVel.y = 0;

        float speedMag = Vector3.Dot(hVel, _inputDir);


        _vel += _inputDir * _airBaseLimit * Time.deltaTime;
    }
    
    private void SwingAccelerate()
    {
        _vel += _inputDir * _swingSpeed * Time.deltaTime;
    }

    private void Accelerate(Vector3 direction, float magnitude, float accelLimit, float acceleration)
    {
        float addSpeed = accelLimit - magnitude;

        if (addSpeed <= 0)
            return;

        float accelSpeed = acceleration * Time.deltaTime;
        
        if (accelSpeed > addSpeed)
            accelSpeed = addSpeed;

        _vel += accelSpeed * direction;
    }
    #endregion

    #region Forces
    private void ApplyFriction(float friction)
    {
        _vel *= Mathf.Clamp01(1 - Time.deltaTime * friction);
    }
    
    private void ApplyGravity()
    {
        _vel.y -= _gravity * Time.deltaTime;
    }

    private void ClampVel(float limit)
    {
        if (_vel.magnitude > limit)
            _vel = _vel.normalized * limit;
    }
    #endregion
    
    #region Mechanics
    
    #region Swing
    [Header("Swing")]
    private SpringJoint _jointL;
    private SpringJoint _jointR;
    [SerializeField] private LayerMask _swingLayerMask;
    [SerializeField] private Vector3 _swingPivot;
    private bool _swingingL;
    private bool _swingingR;

    [SerializeField] private Vector3 _achorOffsetL = new Vector3(-0.1f, 1.1f, 0);
    [SerializeField] private Vector3 _achorOffsetR = new Vector3(0.1f, 1.1f, 0);
    [SerializeField] private float _maxRaycastDistance = 5;
    [SerializeField] private float _minSwingDistance = 0;
    [SerializeField] private float _maxSwingDistance;
    [SerializeField] private float _spring = 4.5f;
    [SerializeField] private float _damper = 7;
    [SerializeField] private float _massScale = 4.5f;
    [SerializeField] private float _swingSpeed = 10;


    public TwoBoneIKConstraint LSillyArm;
    public TwoBoneIKConstraint RSillyArm;

    public TwoBoneIKConstraint LSwingArm;
    public TwoBoneIKConstraint RSwingArm;

    public Transform LSwingTarget;
    public Transform RSwingTarget;

    [SerializeField] private Vector3 _offsetRotationL = Vector3.zero;
    [SerializeField] private Vector3 _offsetRotationR = Vector3.zero;

    private void StartSwing(int armIndex)
    {
        if(_swingingL && armIndex == 0)
            return;

        if(_swingingR && armIndex == 1)
            return;

        // if(Physics.SphereCast(_ref.PlayerCamera.transform.position, 0.25f, _ref.PlayerCamera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
        if(Physics.Raycast(_ref.PlayerCamera.transform.position, _ref.PlayerCamera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
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

    #endregion

    #region Jump
    private void Jump()
    {
        if (!_ableToJump)
            return;

        if (_vel.y < 0f || !_additiveJump)
            _vel.y = 0f;

        _vel += _ref.PlayerTransform.up * _jumpHeight;

        _ref.SimpleCollider.IsGrounded = false;
        _ref.CustomInput._jumpPending = false;

        StartCoroutine(JumpTimer());
    }
    
    public void Jump(Vector3 dir, float multiplier = 1)
    {
        if (!_ableToJump)
            return;

        _vel = dir * (_jumpHeight * multiplier);
        // _ableToClimb = false;

        _ref.CustomInput._jumpPending = false;

        StartCoroutine(JumpTimer());
    }

    private IEnumerator JumpTimer()
    {
        _ableToJump = false;
        yield return new WaitForSeconds(0.1f);
        _ableToJump = true;
    }
    #endregion

    #region Duck
    // private void Duck()
    // {
    //     if(Ducking && Grounded)
    //         ClampVel(_duckBaseLimit);
        
    //     if(!_duringCrouch)
    //         StartCoroutine(ScaleCollider());
    // }

    // private IEnumerator ScaleCollider()
    // {
    //     if(!_player.CustomInput.IsDucking && _player.CylinderCollider.SphereCastHead())
    //         yield break;

    //     _duringCrouch = true;

    //     float t = 0;
    //     float totalTime = 0.2f;
    //     float targetHeight = _player.CustomInput.IsDucking ? _duckColliderHeight : _standColliderHeight;
    //     float height = _player.CylinderCollider.Cylinder.transform.localScale.y;

    //     while (height != targetHeight)
    //     {
    //         height = Mathf.Lerp(height, targetHeight, t/totalTime);
    //         _player.CylinderCollider.SetHeight(height);
            
    //         t += Time.deltaTime;
    //         yield return new WaitForEndOfFrame();
    //     }

    //     _player.CylinderCollider.SetHeight(targetHeight);
    //     _duringCrouch = false;
    // }
    #endregion
    
    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        // If we are in play mode
        if(!Application.isPlaying)
            return;

        Gizmos.color = Color.white;
        // if(Physics.SphereCast(_ref.PlayerCamera.transform.position, 0.25f, _ref.PlayerCamera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
        if(Physics.Raycast(_ref.PlayerCamera.transform.position, _ref.PlayerCamera.transform.forward, out RaycastHit hit, _maxRaycastDistance, _swingLayerMask))
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
            
            // Get joint.anchor world space
            // Vector3 anchor = 

            Gizmos.DrawLine(_ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetR), _jointR.connectedAnchor);
            Gizmos.DrawWireSphere(_jointR.connectedAnchor, 0.1f);
            Gizmos.DrawWireSphere(_ref.PlayerTransform.position + (_ref.PlayerTransform.rotation * _achorOffsetR), 0.1f);
        }
        
        // if(Input.GetMouseButton(0))
        // {
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawRay(_ref.PlayerCamera.transform.position, _ref.PlayerCamera.transform.forward * 5);
        //     Gizmos.DrawWireSphere(_ref.PlayerCamera.transform.position + (_ref.PlayerCamera.transform.forward * 5), 0.25f);
        // }

        // if(Input.GetMouseButton(1))
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawRay(_ref.PlayerCamera.transform.position, _ref.PlayerCamera.transform.forward * 5);
        //     Gizmos.DrawWireSphere(_ref.PlayerCamera.transform.position + (_ref.PlayerCamera.transform.forward * 5), 0.25f);
        // }
        
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawRay(_player.PlayerTransform.position, Normal);

        //     Gizmos.color = Color.red;
        //     Gizmos.DrawRay(_player.PlayerTransform.position, right);

        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawRay(_player.PlayerTransform.position, forward);


        //     Gizmos.color = Color.white;
        //     Gizmos.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.up);
        //     Gizmos.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.forward);
    }
    #endregion
}