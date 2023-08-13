using System;
using UnityEngine;

[Serializable]
public partial class Movement
{
    [Header("Acceleration")]
    [SerializeField] private readonly float _groundAcceleration = 100f;
    [SerializeField] private readonly float _groundBaseLimit = 12f;

    [SerializeField] private readonly float _airAcceleration = 100f;
    [SerializeField] private readonly float _airBaseLimit = 1f;

    // [SerializeField] private float _duckAcceleration = 6f;
    // [SerializeField] private float _duckBaseLimit = 6f;

    
    [Header("Forces")]
    [SerializeField] private readonly float _gravity = 16f;
    [SerializeField] private readonly float _friction = 6f;
    [SerializeField] private readonly float _jumpHeight = 6f;

    // [Header("Collider")]
    // [SerializeField] private float _duckColliderHeight = 0.6f;
    // [SerializeField] private float _standColliderHeight = 1f;

    [Header("Hover Threshold")]
    [SerializeField] private readonly float thresholdBot = 1.19f;
    [SerializeField] private readonly float thresholdTop = 1.2f;

    [Header("Movement Toggles")]
    [SerializeField] private bool _flyToggle = true;

    [SerializeField] private readonly bool _additiveJump = true;
    [SerializeField] private readonly bool _clampGroundSpeed = false;
    // [SerializeField] private bool _disableBunnyHopping = false;


    #region Global Variables
    private Player _player;

    MovementState _state = MovementState.Air;
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

    // Fly
    private bool _fly = false;

    // Jump
    private bool _ableToJump = true;

    // Climb
    private bool _ableToClimb = true;

    // Duck
    // private bool _duringCrouch = false;

    // Boolean Properties
    private bool JumpPending => _player.CustomInput.JumpPending;
    private bool Ducking => _player.CustomInput.DuckingPending;
    #endregion
}