using UnityEngine;

public class CrocMovement : MonoBehaviour
{
    private Transform _transform;
    private CrocRefrence _crocRefrence;
    private Vector3 _targetDir;
    private Vector3 _vel;

    [SerializeField] private float _groundBaseLimit = 10;
    [SerializeField] private float _groundAcceleration = 20;
    [SerializeField] private bool _clampGroundSpeed = false;

    public enum CrocMove
    {
        Ground, 
        Air,
        Swim // Boosted jump strength, death roll, bring food to water
    }

    private void Awake()
    {
        _transform = transform;
        _crocRefrence = GetComponent<CrocRefrence>();
    }

    void Update()
    {
        // _targetDir = _behaviour.TargetDir;
        // _targetDir.y = _transform.position.y;
        // _targetDir.Normalize();
    }

    void FixedUpdate()
    {
        _vel = _crocRefrence.Rigidbody.velocity;

        Vector3 right = Vector3.Cross(_crocRefrence.RaycastGround.AdvNormal, _transform.forward);
        Vector3 forward = Vector3.Cross(right, _crocRefrence.RaycastGround.AdvNormal);

        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(forward, _crocRefrence.RaycastGround.AdvNormal), Time.deltaTime * 5f);
        
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

        _crocRefrence.Rigidbody.velocity = _vel;
    }

    private CrocMove GetMoveState()
    {
        if(_crocRefrence.RaycastGround.IsGrounded)
            return CrocMove.Ground;
        else
            return CrocMove.Air;
    }

    private void GroundMovement()
    {
        if(_targetDir == Vector3.zero)
            return;

        _targetDir = Vector3.Cross(Vector3.Cross(_crocRefrence.RaycastGround.AdvNormal, _targetDir), _crocRefrence.RaycastGround.AdvNormal).normalized;

        Quaternion targetRot = Quaternion.LookRotation(_targetDir, _crocRefrence.RaycastGround.AdvNormal);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, targetRot, Time.deltaTime * 5f);

        // if(Vector3.Distance(_transform.position, _behaviour.Destination) > 2)
        
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
}