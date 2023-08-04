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

    // [SerializeField] private float _duckAcceleration = 6f;
    // [SerializeField] private float _duckBaseLimit = 6f;

    
    [Header("Forces")]
    [SerializeField] private float _gravity = 16f;
    [SerializeField] private float _friction = 6f;
    [SerializeField] private float _jumpHeight = 6f;


    // [Header("Collider")]
    // [SerializeField] private float _duckColliderHeight = 0.6f;
    // [SerializeField] private float _standColliderHeight = 1f;


    [Header("Movement Toggles")]
    [SerializeField] private bool _flyToggle = true;
    // [SerializeField] private bool _jumpToggle = true;
    // [SerializeField] private bool _duckToggle = true;
    // [SerializeField] private bool _airControl = true;

    [SerializeField] private bool _additiveJump = true;
    [SerializeField] private bool _autoJump = true;
    [SerializeField] private bool _clampGroundSpeed = false;
    // [SerializeField] private bool _disableBunnyHopping = false;


    #region Global Variables
    private Player _player;

    // MovementState _state = MovementState.Air;
    public enum MovementState
    {
        Ground,
        Air,
        Fly,
        Ragdoll
    }

    [SerializeField] private bool Grounded => _player.PlayerCollider.IsGrounded;
    [SerializeField] private Vector3 Normal => _player.PlayerCollider.AdverageNormal;

    // Input
    private Vector3 _inputDir;

    // Velocity
    private Vector3 _vel;
    public Vector3 Velocity { get => _vel; }
    private float _currentSpeed = 0f;
    public float CurrentSpeed { get => _currentSpeed; }

    // Fly
    private bool _fly = false;

    // Jump
    private bool _ableToJump = true;

    // Duck
    // private bool _duringCrouch = false;

    // Boolean Properties
    private bool JumpPending => _player.CustomInput.JumpPending;
    private bool Ducking => _player.CustomInput.DuckingPending;
    #endregion
}