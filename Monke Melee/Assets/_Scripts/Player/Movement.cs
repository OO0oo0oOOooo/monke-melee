using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public partial class Movement : MonoBehaviour
{
    #region Unity Event Functions
    
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    Vector3 right;
    Vector3 forward;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
            _fly = !_fly;

        if(_player.CustomInput.Mouse2Pending)
            _inputDir = _player.CustomInput.InputDirPlayer;
        else
            _inputDir = _player.CustomInput.InputDirCamera;

        right = Vector3.Cross(Normal, _player.PlayerTransform.forward);
        forward = Vector3.Cross(right, Normal);

        // Rotate player to look in movement direction
        if(!_player.CustomInput.Mouse2Pending)
            if(_vel.magnitude > 0.1f)
                _player.PlayerTransform.localRotation = Quaternion.Lerp(_player.PlayerTransform.localRotation,  Quaternion.LookRotation(_vel.normalized, _player.PlayerTransform.up), Time.deltaTime * 10f);

        // Rotate Player Transform to align with surface
        if(Normal != Vector3.zero)
            _player.PlayerTransform.rotation = Quaternion.Lerp(_player.PlayerTransform.rotation, Quaternion.LookRotation(forward, Normal), Time.deltaTime * 20f);

        // Hover player above ground
        if(Grounded && _player.PlayerCollider.AdverageDistance < thresholdBot)
            _player.PlayerTransform.position += Normal * 0.01f;

        if(Grounded && _player.PlayerCollider.AdverageDistance > thresholdTop)
            _player.PlayerTransform.position -= Normal * 0.01f;

        // Reset able to climb after landing
        if(!_ableToClimb && _player.PlayerCollider.AdverageDistance < thresholdBot)
            _ableToClimb = true;

        // Set animator values
        _player.Animator.SetFloat("Velocity", _currentSpeed);
    }
    
    private void FixedUpdate()
    {
        // Sync before changing
        _vel = _player.Rigidbody.velocity;

        // Measure Speed
        _currentSpeed = _player.Rigidbody.velocity.magnitude;

        // ClampVel(_groundBaseLimit);

        if (JumpPending && Grounded)
            Jump(_player.PlayerCamera.transform.forward);


        switch (GetMovementState())
        {
            case MovementState.Ground:
                Ground();
                break;
            case MovementState.Air:
                Air();
                break;
            case MovementState.Fly:
                Fly();
                break;
            default:
                break;
        }
        
        // Apply changes
        _player.Rigidbody.velocity = _vel;
    }
    #endregion

    private MovementState GetMovementState()
    {
        if(_flyToggle && _fly)
            return MovementState.Fly;

        if(Grounded && _ableToClimb)
            return MovementState.Ground;
        else
            return MovementState.Air;
    }

    private void Ground()
    {
        _inputDir = Vector3.Cross(Vector3.Cross(Normal, _inputDir), Normal).normalized;

        _vel = _inputDir * 10f;
        ApplyFriction(_friction);
    }
    
    private void Air()
    {
        ApplyGravity();
    }

    private void Fly()
    {
        float y;
        if(JumpPending)
            y = 1;
        else if(_player.CustomInput.DuckingPending)
            y = -1;
        else
            y = 0;

        // _vel += ((_cam.transform.forward * CustomInput.Instance.InputRaw.y) + (Vector3.up * y) + (_cam.transform.right * CustomInput.Instance.InputRaw.x)) * 10f;
        Vector3 flyDir = _player.PlayerCamera.transform.rotation * new Vector3(_player.CustomInput.InputAxisRaw.x, y, _player.CustomInput.InputAxisRaw.z);

        _vel += flyDir * 10f;
        ApplyFriction(_friction);
    }

    #region Acceleration
    // private void GroundAccelerate()
    // {
    //     float speedMag = Vector3.Dot(_vel, _inputDir);
    //     Accelerate(_inputDir, speedMag, _groundBaseLimit, _groundAcceleration);

    //     if (_clampGroundSpeed)
    //         ClampVel(_groundBaseLimit);
    // }

    // private void AirAccelerate()
    // {
    //     Vector3 hVel = _vel;
    //     hVel.y = 0;

    //     float speedMag = Vector3.Dot(hVel, _inputDir);
    //     Accelerate(_inputDir, speedMag, _airBaseLimit, _airAcceleration);
    // }

    // private void Accelerate(Vector3 direction, float magnitude, float accelLimit, float accelerationType)
    // {
    //     float addSpeed = accelLimit - magnitude;

    //     if (addSpeed <= 0)
    //         return;

    //     float accelSpeed = accelerationType * Time.deltaTime;
        
    //     if (accelSpeed > addSpeed)
    //         accelSpeed = addSpeed;

    //     _vel += accelSpeed * direction;
    // }
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
    // private void Jump()
    // {
    //     if (!_ableToJump)
    //         return;

    //     if (_vel.y < 0f || !_additiveJump)
    //         _vel.y = 0f;

    //     _vel += _player.PlayerTransform.up * _jumpHeight;
    //     _ableToClimb = false;
    //     // _player.CylinderCollider.OnGround = false;
    //     // _player.PlayerCollider.Grounded = false;

    //     _player.CustomInput._jumpPending = false;

    //     StartCoroutine(JumpTimer());
    // }
    
    private void Jump(Vector3 dir, float multiplier = 1)
    {
        if (!_ableToJump)
            return;

        _vel = dir * (_jumpHeight * multiplier);
        _ableToClimb = false;

        _player.CustomInput._jumpPending = false;

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
    //     if(Ducking && _player.CylinderCollider.OnGround)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_player.PlayerTransform.position, Normal);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_player.PlayerTransform.position, right);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_player.PlayerTransform.position, forward);


        Gizmos.color = Color.white;
        Gizmos.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.up);
        Gizmos.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.forward);
    }
}