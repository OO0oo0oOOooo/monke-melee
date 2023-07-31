using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(CylinderCollider))]
public partial class Movement : NetworkBehaviour
{
    #region Unity Event Functions
    // public override void OnNetworkSpawn()
    // {
    //     if(!IsOwner) return;

    //     _cam.gameObject.SetActive(true);
    // }

    private void Awake()
    {
        // if (!IsOwner) return;

        _transform = transform;
        _customInput = GetComponent<CustomInput>();
        _rb = GetComponent<Rigidbody>();
        _collision = GetComponent<CylinderCollider>();
        _cam = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (!IsSpawned) return;
        if (!IsOwner) return;

        _inputDir = _customInput.InputDir;
    }
    
    private void FixedUpdate()
    {
        // if (!IsSpawned) return;
        // if (!IsOwner) return;

        // Sync before changing
        _vel = _rb.velocity;

        // Measure Speed
        _currentSpeed = (_transform.position - _lastPosition).magnitude / Time.deltaTime;
        _lastPosition = _transform.position;

        // Clamp speed if bunnyhopping is disabled
        if (_disableBunnyHopping && _collision.OnGround)
            ClampVel(_groundBaseLimit);

        if (JumpPending)
            Jump();

        Duck();
        
        // We use air physics if moving upwards at high speed
        if (_rampSlideLimit >= 0f && _vel.y > _rampSlideLimit)
            _collision.OnGround = false;

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
        _rb.velocity = _vel;

        // Reset onGround before next collision checks
        _collision.OnGround = false;
        _collision.GroundNormal = Vector3.zero;

        // Set OnWall to false
    }
    #endregion

    private MovementState GetMovementState()
    {
        // if(_fly && _flyToggle)
        //     return MovementState.FLY;

        if(_collision.OnGround)
            return MovementState.Ground;
        else
            return MovementState.Air;
    }
    
    private void Ground()
    {
        // Rotate movement vector to match ground tangent
        _inputDir = Vector3.Cross(Vector3.Cross(_collision.GroundNormal, _inputDir), _collision.GroundNormal);

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
        _collision.OnGround = false;

        if (!_autoJump)
            _customInput._jumpPending = false;

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
        if(Ducking && _collision.OnGround)
            ClampVel(_duckBaseLimit);
        
        if(!duringCrouch)
            StartCoroutine(ScaleCollider());
    }

    private IEnumerator ScaleCollider()
    {
        if(!_customInput.IsDucking && _collision.SphereCastHead())
            yield break;

        duringCrouch = true;

        float t = 0;
        float totalTime = 0.2f;
        float targetHeight = _customInput.IsDucking ? _duckColliderHeight : _standColliderHeight;
        float height = _collision.Cylinder.transform.localScale.y;

        while (height != targetHeight)
        {
            height = Mathf.Lerp(height, targetHeight, t/totalTime);
            _collision.SetHeight(height);
            
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _collision.SetHeight(targetHeight);
        duringCrouch = false;
    }
    #endregion
    #endregion
}
