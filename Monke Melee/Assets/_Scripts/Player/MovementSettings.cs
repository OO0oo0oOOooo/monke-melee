using System;
using UnityEngine;

[Serializable]
public partial class Movement
{
    [Header("Acceleration")]
    [SerializeField] private float _groundAcceleration = 100f;
    [SerializeField] private float _groundBaseLimit = 12f;

    [SerializeField] private float _airAcceleration = 100f;
    [SerializeField] private float _airBaseLimit = 1f;

    [SerializeField] private float _duckAcceleration = 6f;
    [SerializeField] private float _duckBaseLimit = 6f;

    [SerializeField] private float _slideAcceleration = 1f;
    [SerializeField] private float _slideBaseLimit = 100f;

    [SerializeField] private float _slideBoostAcceleration = 10;
    
    [Header("Forces")]
    [SerializeField] private float _gravity = 16f;
    [SerializeField] private float _friction = 6f;
    [SerializeField] private float _jumpHeight = 6f;
    [SerializeField] private float _rampSlideLimit = 5f;

    [Header("Slide")]
    [SerializeField] private float _slideStartThreshold = 8f;
    [SerializeField] private float _slideEndThreshold = 3f;
    [SerializeField] private float _slideFriction = 0.7f;
    [SerializeField] private float _slideFrictionThreshold = 0.1f;
    [SerializeField] private float _slideBoostCooldown = 3;
    [SerializeField] private float _slideBoostDuration = 0.5f;
    [SerializeField] private AnimationCurve SlideBoostCurve;

    [Header("Collider")]
    [SerializeField] private float _duckColliderHeight = 0.6f;
    [SerializeField] private float _standColliderHeight = 1f;

    [Header("Movement Toggles")]
    [SerializeField] private bool _additiveJump = true;
    [SerializeField] private bool _autoJump = true;
    [SerializeField] private bool _clampGroundSpeed = false;
    [SerializeField] private bool _disableBunnyHopping = false;

    #region Global Variables

    [SerializeField] private Transform _transform;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CustomInput _customInput;
    [SerializeField] private CylinderCollider _collision;
    [SerializeField] private Camera _cam;

    MovementState _state = MovementState.Air;
    public enum MovementState
    {
        Ground,
        Air,
        Climb,
        Ragdoll
    }

    // Input
    private Vector3 _inputDir;

    // 
    private Vector3 _vel;
    public Vector3 Velocity { get => _vel; }
    private float _currentSpeed = 0f;
    public float CurrentSpeed { get => _currentSpeed; }
    private Vector3 _lastPosition;

    // Jump
    private bool _ableToJump = true;

    // Duck
    private bool duringCrouch = false;


    // Boolean Properties
    private bool JumpPending => _customInput._jumpPending && _collision.OnGround;
    private bool Ducking => _customInput.IsDucking;

    #endregion
}