using System.Collections;
using UnityEngine;

public class Croc : MonoBehaviour
{
    private Rigidbody _rb;
    private CrocCollision _collision;
    private Transform _transform;

    [SerializeField] private Transform _target;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpHeight = 50;

    [SerializeField] private float _groundBaseLimit = 10;
    [SerializeField] private float _groundAcceleration = 20;
    [SerializeField] private bool _clampGroundSpeed = false;

    [SerializeField] private float _trackRange = 25;
    [SerializeField] private float _jumpRange = 10;

    private bool _ableToJump = true;
    private Vector3 _vel;
    private Vector3 _destination;
    private Vector3 _targetDir;
    private float _targetDistance;

    // private CrocBehaviour _behaviourState = CrocBehaviour.Idle;
    public enum CrocBehaviour
    {
        Idle,   // float and sunbathe
        Wander, // Wander around

        Search, // Search for player
        Hunt,   // Hide under water and wait for player to get close
        Chase,  // get that ****** and eat him

        Eat     // bring player to water and death roll/eat
    }

    // private CrocMove _moveState = CrocMove.Ground;
    public enum CrocMove
    {
        Ground, 
        Air,    
        Swim    // Boosted jump strength, death roll, bring food to water
    }

    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
        _collision = GetComponent<CrocCollision>();
    }

    private void Start()
    {
        _destination = _transform.position;
    }

    void Update()
    {
        _targetDir = (_destination - _transform.position).normalized;

        switch (GetBehaviourState())
        {
            case CrocBehaviour.Idle:
                Idle();
                break;
            case CrocBehaviour.Chase:
                Chase();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        // Do crocadile things like float and sunbathe
    }

    private void Chase()
    {
        // Get distance to target
        // If target leaves range then set target to null
        // If target is in range then set targetDir

        _targetDistance = Vector3.Distance(_target.position, _transform.position);
        
        if(_targetDistance > _trackRange)
            _target = null;

        // Raycast toward target and check if we have line of sight. If not then set target to null
        if(_targetDistance < _trackRange)
        {
            _destination = _target.position;
        }

    }

    void FixedUpdate()
    {
        _vel = _rb.velocity;

        // if(_targetDistance < _jumpRange && _ableToJump)
        //     JumpAttack();
        Vector3 right = Vector3.Cross(_collision.AdvNormal, _transform.forward);
        Vector3 forward = Vector3.Cross(right, _collision.AdvNormal);

        if(Vector3.Dot(_transform.up, Vector3.up) < 0.5f)
        {
            _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(_transform.up, Vector3.up), Time.deltaTime * 5f);
        }
        else
        {
        }
        _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(forward, _collision.AdvNormal), Time.deltaTime * 5f);
        
        switch (GetMoveState())
        {
            case CrocMove.Ground:
                GroundMovement();
                break;
            case CrocMove.Air:
                AirMovement();
                break;
            case CrocMove.Swim:
                SwimMovement();
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

        if(_collision.IsGrounded)
            return CrocMove.Ground;
        else
            return CrocMove.Air;
    }

    private CrocBehaviour GetBehaviourState()
    {
        if(_target == null)
            return CrocBehaviour.Idle;
        else
            return CrocBehaviour.Chase;
    }

    private void GroundMovement()
    {
        _targetDir = Vector3.Cross(Vector3.Cross(_collision.AdvNormal, _targetDir), _collision.AdvNormal).normalized;

        Quaternion targetRot = Quaternion.LookRotation(_targetDir, _collision.AdvNormal);
        _transform.rotation = Quaternion.Lerp(_transform.rotation, targetRot, Time.deltaTime * 5f);

        if(Vector3.Distance(_transform.position, _destination) > 1)
            GroundAccelerate();
        
        ApplyFriction(8);
    }

    private void AirMovement()
    {
        ApplyGravity();
        // _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(_vel.normalized, Vector3.up), Time.deltaTime * 5f);
    }

    private void SwimMovement()
    {
        _vel += _moveSpeed * Time.deltaTime * _targetDir;
    }

    private void ApplyGravity()
    {
        _vel.y -= 16 * Time.deltaTime;
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

    private void GroundAccelerate()
    {
        float speedMag = Vector3.Dot(_vel, _targetDir);
        Accelerate(_targetDir, speedMag, _groundBaseLimit, _groundAcceleration);

        if (_clampGroundSpeed)
            ClampVel(_groundBaseLimit);
    }
    
    private void SwimAccelerate()
    {

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


    void OnTriggerStay(Collider other)
    {
        // Check which trigger is triggering this
        
        // Check line of sight for all players within range
        // Set Destination to closest player.
        // If no line of sight move to last known position.

        if(other.CompareTag("Player"))
        {
            _target = other.gameObject.transform;
        }
    }
}