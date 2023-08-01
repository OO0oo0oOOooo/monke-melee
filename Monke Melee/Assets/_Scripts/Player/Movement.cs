using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(CylinderCollider))]
public partial class Movement : MonoBehaviour
{
    #region Unity Event Functions
    
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _inputDir = _player.CustomInput.InputDir;

        if(Velocity.magnitude > 0.1f)
            _player.PlayerTransform.rotation = Quaternion.LookRotation(_vel.normalized, Vector3.up);

        // _player.PlayerTransform.rotation = Quaternion.Euler(0f, _player.CustomInput.InputRot.y, 0f);
        _player.Animator.SetFloat("Velocity", _currentSpeed);
    }
    
    private void FixedUpdate()
    {
        // Sync before changing
        _vel = _player.Rigidbody.velocity;

        // Measure Speed
        _currentSpeed = _player.Rigidbody.velocity.magnitude;

        // Clamp speed if bunnyhopping is disabled
        if (_disableBunnyHopping && _player.CylinderCollider.OnGround)
            ClampVel(_groundBaseLimit);

        if (JumpPending)
            Jump();

        Duck();
        
        // We use air physics if moving upwards at high speed
        if (_rampSlideLimit >= 0f && _vel.y > _rampSlideLimit)
            _player.CylinderCollider.OnGround = false;

        switch (GetMovementState())
        {
            case MovementState.Ground:
                Ground();
                break;
            case MovementState.Air:
                Air();
                break;
            case MovementState.Climb:
                Climb();
                break;
            // case MovementState.LEDGE:
            //     Ledge();
            //     break;
            // case MovementState.FLY:
            //     FlyPhysics();
            //     break;
            default:
                break;
        }
        
        // Apply changes
        _player.Rigidbody.velocity = _vel;

        // Reset onGround before next collision checks
        _player.CylinderCollider.OnGround = false;
        _player.CylinderCollider.GroundNormal = Vector3.zero;

        // Set OnWall to false
    }
    #endregion

    private MovementState GetMovementState()
    {
        // if(_fly && _flyToggle)
        //     return MovementState.FLY;

        if(_player.CylinderCollider.OnGround)
            return MovementState.Ground;
        else
            return MovementState.Air;
    }
    
    private void Ground()
    {
        // Rotate movement vector to match ground tangent
        _inputDir = Vector3.Cross(Vector3.Cross(_player.CylinderCollider.GroundNormal, _inputDir), _player.CylinderCollider.GroundNormal);

        GroundAccelerate();
        ApplyFriction(_friction);
    }
    
    private void Air()
    {
        AirAccelerate();
        ApplyGravity();
    }

    private void Climb()
    {
        Debug.Log("Climb");
    }

    private void FlyPhysics()
    {
        Debug.Log("Fly Physics");
    }

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

    private void Accelerate(Vector3 direction, float magnitude, float accelLimit, float accelerationType)
    {
        float addSpeed = accelLimit - magnitude;

        if (addSpeed <= 0)
            return;

        float accelSpeed = accelerationType * Time.deltaTime;
        
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

        _vel.y += _jumpHeight;
        _player.CylinderCollider.OnGround = false;

        if (!_autoJump)
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
    private void Duck()
    {
        if(Ducking && _player.CylinderCollider.OnGround)
            ClampVel(_duckBaseLimit);
        
        if(!duringCrouch)
            StartCoroutine(ScaleCollider());
    }

    private IEnumerator ScaleCollider()
    {
        if(!_player.CustomInput.IsDucking && _player.CylinderCollider.SphereCastHead())
            yield break;

        duringCrouch = true;

        float t = 0;
        float totalTime = 0.2f;
        float targetHeight = _player.CustomInput.IsDucking ? _duckColliderHeight : _standColliderHeight;
        float height = _player.CylinderCollider.Cylinder.transform.localScale.y;

        while (height != targetHeight)
        {
            height = Mathf.Lerp(height, targetHeight, t/totalTime);
            _player.CylinderCollider.SetHeight(height);
            
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _player.CylinderCollider.SetHeight(targetHeight);
        duringCrouch = false;
    }
    #endregion
    #endregion
}
