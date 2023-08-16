using System.Collections;
using UnityEngine;

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

        _inputDir = _ref.CustomInput.InputDirPlayer;

        _state =  GetMovementState();

        /* 
        switch (_state)
        {
            // case MovementState.Ground:
            //     UpdateGround();
            //     break;
            // case MovementState.Air:
            //     UpdateAir();
            //     break;
            // case MovementState.Swing:
            //     UpdateSwing();
            //     break;
            default:
                break;
        }
        */
    }
    
    private void FixedUpdate()
    {
        _vel = _ref.Rigidbody.velocity;

        if (JumpPending && Grounded)
            Jump();

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

        if(_ref.GibbonSwing.SwingingL || _ref.GibbonSwing.SwingingR)
            return MovementState.Swing;

        if(Grounded)
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
        Vector3 flyDir = _ref.Camera.transform.rotation * new Vector3(_ref.CustomInput.InputAxisRaw.x, y, _ref.CustomInput.InputAxisRaw.z);

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
    
    private void SwingAccelerate()
    {
        _vel += _inputDir * _swingAcceleration * Time.deltaTime;
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
        // Gizmos.color = Color.green;
        // Gizmos.DrawRay(_player.PlayerTransform.position, Normal);

        // Gizmos.color = Color.red;
        // Gizmos.DrawRay(_player.PlayerTransform.position, right);

        // Gizmos.color = Color.blue;
        // Gizmos.DrawRay(_player.PlayerTransform.position, forward);


        // Gizmos.color = Color.white;
        // Gizmos.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.up);
        // Gizmos.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.forward);
    }
    #endregion
}