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

    private void Update()
    {
        _inputDir = _player.CustomInput.InputDirCamera;

        if(Input.GetKeyDown(KeyCode.X))
            _fly = !_fly;

        // Rotate model to look in movement direction
        if(_vel.magnitude > 0.1f)
            _player.PlayerTransform.localRotation = Quaternion.LookRotation(_vel.normalized, _player.PlayerTransform.up);

        Vector3 right = Vector3.Cross(Normal, _player.PlayerTransform.forward);
        Vector3 forward = Vector3.Cross(right, Normal);

        // Rotate Player Transform to align with surface
        if(Normal != Vector3.zero)
            _player.PlayerTransform.rotation = Quaternion.LookRotation(forward, Normal);

        // Set the player position.y to the adverage point.y
        if(Grounded)
            _player.PlayerTransform.localPosition = new Vector3(_player.PlayerTransform.position.x, _player.PlayerCollider.AdveragePoint.y + 1.2f, _player.PlayerTransform.position.z);
        

        // Debug.DrawRay(_player.PlayerTransform.position, _normal, Color.green);
        // Debug.DrawRay(_player.PlayerTransform.position, right, Color.red);
        // Debug.DrawRay(_player.PlayerTransform.position, forward, Color.blue);

        Debug.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.up, Color.white);
        Debug.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.forward, Color.white);

        // Set animator values
        _player.Animator.SetFloat("Velocity", _currentSpeed);
    }
    
    private void FixedUpdate()
    {
        // Sync before changing
        _vel = _player.Rigidbody.velocity;

        // Measure Speed
        _currentSpeed = _player.Rigidbody.velocity.magnitude;

        ClampVel(_groundBaseLimit);

        if (JumpPending)
            Jump();

        switch (GetMovementState())
        {
            case MovementState.Ground:
                Ground();
                break;
            case MovementState.Air:
                Air();
                break;
            case MovementState.Fly:
                FlyPhysics();
                break;
            default:
                break;
        }
        
        // Apply changes
        _player.Rigidbody.velocity = _vel;

        _player.PlayerCollider.OnGround = false;
        _player.PlayerCollider.AdveragePoint = Vector3.zero;
        // _player.PlayerCollider.IsGrounded = false;
        // _player.PlayerCollider.AdverageNormal = Vector3.up;
    }
    #endregion

    private MovementState GetMovementState()
    {
        if(_flyToggle && _fly)
            return MovementState.Fly;

        if(Grounded)
            return MovementState.Ground;
        else
            return MovementState.Air;
    }

    private void Ground()
    {
        _inputDir = Vector3.Cross(Vector3.Cross(Normal, _inputDir), Normal);

        GroundAccelerate();
        ApplyFriction(_friction);
    }
    
    private void Air()
    {
        // AirAccelerate();
        ApplyGravity();
    }

    private void FlyPhysics()
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

        _vel += _player.PlayerTransform.up * _jumpHeight;
        // _player.CylinderCollider.OnGround = false;
        _player.PlayerCollider.IsGrounded = false;

        _player.CustomInput._jumpPending = false;

        StartCoroutine(JumpTimer());
    }
    
    private void JumpDirectional(Vector3 dir, float strength = 1, bool airAutoJump = false, bool rotatePlayerOnJump = true, bool additiveJump = true)
    {
        if (!_ableToJump)
            return;

        if (_vel.y < 0f || !additiveJump)
            _vel.y = 0f;

        _vel += dir * (_jumpHeight * strength);

        if(rotatePlayerOnJump)
            _player.PlayerTransform.eulerAngles = new Vector3(0, _player.PlayerCamera.transform.eulerAngles.y, 0);

        if(!airAutoJump)
            _player.CustomInput._jumpPending = false;

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
}


// private void Climb()
    // {
    //     _player.CylinderCollider.RaycastWall(ClimbRaycastDistance);

    //     // check the distance to the wall and move the _player.PlayerTransform forward if it's too far
    //     if(_player.CylinderCollider.FrontWallHit.distance > ClimbMinPlayerDistance && _player.CylinderCollider.UpWallHit.distance > ClimbMinPlayerDistance && _player.CylinderCollider.DownWallHit.distance > ClimbMinPlayerDistance && _player.CylinderCollider.RightWallHit.distance > ClimbMinPlayerDistance && _player.CylinderCollider.LeftWallHit.distance > ClimbMinPlayerDistance)
    //         _player.PlayerTransform.position += _player.PlayerTransform.forward * ClimbAdjustPlayerDistanceSpeed;

    //     // if(JumpPending)
    //     // {
    //         // JumpDirectional(_player.PlayerCamera.transform.forward, 1);
    //         // // Climbing = false;
    //     // }

    //     Vector3 point = _player.CylinderCollider.FrontWallHit.point;
    //     // Vector3 normal = _player.CylinderCollider.FrontWallHit.normal;

    //     // Average normal of all wall hits that are true
    //     Vector3 normal = ((_player.CylinderCollider.FrontWallHit.normal + _player.CylinderCollider.UpWallHit.normal + _player.CylinderCollider.DownWallHit.normal + _player.CylinderCollider.RightWallHit.normal + _player.CylinderCollider.LeftWallHit.normal) / 5f).normalized;
    //     Vector3 right = Vector3.Cross(normal, _player.PlayerTransform.up);
    //     Vector3 forward = Vector3.Cross(right, normal);
        
    //     Debug.DrawRay(point, normal, Color.green);
    //     Debug.DrawRay(point, right, Color.red);
    //     Debug.DrawRay(point, forward, Color.blue);

    //     float climbRot = 0;
    //     float climbRotSpeed = 400 * Time.deltaTime;
        
    //     if(Input.GetKey(KeyCode.Q))
    //         climbRot += climbRotSpeed;
    //     if(Input.GetKey(KeyCode.E))
    //         climbRot -= climbRotSpeed;

    //     Quaternion wallRotation = Quaternion.LookRotation(-normal, _player.PlayerTransform.up) * Quaternion.Euler(0, 0, climbRot);

    //     // Lerp rotation to align player to wall
    //     _player.PlayerTransform.rotation = Quaternion.Lerp(_player.PlayerTransform.rotation, wallRotation, Time.deltaTime * 10f);

    //     float xInput = _player.CustomInput.InputAxisRaw.x;
    //     float yInput = _player.CustomInput.InputAxisRaw.z;

    //     // Debug.DrawRay(_player.PlayerTransform.position + (_player.PlayerTransform.up * 1f) + (-_player.PlayerTransform.forward * 0.5f), _player.PlayerTransform.forward * 1.5f, Color.yellow);
    //     // if(!Physics.Raycast(_player.PlayerTransform.position + (_player.PlayerTransform.up * 1f) + (-_player.PlayerTransform.forward * 0.5f), _player.PlayerTransform.forward, out RaycastHit hitWallAbove, 1.5f) && yInput > 0)
    //     // {
    //     //     yInput = 0;
    //     //     _vel.y = 0;
    //     // }

    //     // Apply movement
    //     _vel += forward * yInput + right * xInput;
    //     ApplyFriction(_friction);
    // }