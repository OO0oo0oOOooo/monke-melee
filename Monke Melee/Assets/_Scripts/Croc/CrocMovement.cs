using System.Collections;
using UnityEngine;

public class CrocMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private RaycastGround _groundCheck;
    private Transform _transform;
    private CrocBehaviour _behaviour;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpHeight = 50;

    [SerializeField] private float _groundBaseLimit = 10;
    [SerializeField] private float _groundAcceleration = 20;
    [SerializeField] private bool _clampGroundSpeed = false;

    [SerializeField] private float _jumpRange = 10;

    private bool _ableToJump = true;
    private Vector3 _vel;
    private Vector3 _targetDir;

    // private CrocMove _moveState = CrocMove.Ground;
    public enum CrocMove
    {
        Ground, 
        Air,
        Swim // Boosted jump strength, death roll, bring food to water
    }

    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
        _groundCheck = GetComponent<RaycastGround>();
        _behaviour = GetComponent<CrocBehaviour>();
    }

    void FixedUpdate()
    {
        _vel = _rb.velocity;

        // if(_targetDistance < _jumpRange && _ableToJump)
        //     JumpAttack();

        Vector3 right = Vector3.Cross(_groundCheck.AdvNormal, _transform.forward);
        Vector3 forward = Vector3.Cross(right, _groundCheck.AdvNormal);

        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(forward, _groundCheck.AdvNormal), Time.deltaTime * 5f);
        
        switch (GetMoveState())
        {
            case CrocMove.Ground:
                GroundMovement();
                break;
            case CrocMove.Air:
                AirMovement();
                break;
            default:
                break;
        }

        _rb.velocity = _vel;
    }

    private CrocMove GetMoveState()
    {
        // check if gater is in water
        // return GaterMove.Swim;

        if(_groundCheck.IsGrounded)
            return CrocMove.Ground;
        else
            return CrocMove.Air;
    }

    private void GroundMovement()
    {
        _targetDir = Vector3.Cross(Vector3.Cross(_groundCheck.AdvNormal, _targetDir), _groundCheck.AdvNormal).normalized;

        Quaternion targetRot = Quaternion.LookRotation(_targetDir, _groundCheck.AdvNormal);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, targetRot, Time.deltaTime * 5f);

        if(Vector3.Distance(_transform.position, _behaviour.Destination) > 1)
            GroundAccelerate();
        
        ApplyFriction(8);
    }

    private void AirMovement()
    {
        ApplyGravity();
        // _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(_vel.normalized, Vector3.up), Time.deltaTime * 5f);
    }

    private void GroundAccelerate()
    {
        float speedMag = Vector3.Dot(_vel, _targetDir);
        Accelerate(_targetDir, speedMag, _groundBaseLimit, _groundAcceleration);

        if (_clampGroundSpeed)
            ClampVel(_groundBaseLimit);
    }

    private void ApplyGravity()
    {
        _vel.y -= 16 * Time.deltaTime;
    }

    private void ApplyFriction(float friction)
    {
        _vel *= Mathf.Clamp01(1 - Time.deltaTime * friction);
    }
    
    private void ClampVel(float limit)
    {
        if (_vel.magnitude > limit)
            _vel = _vel.normalized * limit;
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

    private void JumpAttack()
    {
        _rb.AddForce(_jumpHeight * _rb.mass * _targetDir);
        StartCoroutine(JumpTimer());
    }

    private IEnumerator JumpTimer()
    {
        _ableToJump = false;
        yield return new WaitForSeconds(5f);
        _ableToJump = true;
    }
}