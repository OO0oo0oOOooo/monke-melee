using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ProceduralAnimation : MonoBehaviour
{
    private GibbonRefrences _ref;
    [SerializeField] private Rigidbody _rigidbody;

    // [SerializeField] private Transform _headTarget;
    // [SerializeField] private Transform _chestTarget;
    // [SerializeField] private Transform _hipTarget;

    #region Walk Parameters
    [Header("Walk Constraints")]
    [SerializeField] private TwoBoneIKConstraint _handWalkConstraintR;
    [SerializeField] private TwoBoneIKConstraint _handWalkConstraintL;

    [SerializeField] private TwoBoneIKConstraint _footWalkConstraintR;
    [SerializeField] private TwoBoneIKConstraint _footWalkConstraintL;

    [Header("Walk Targets")]
    [SerializeField] private Transform _handWalkTargetR;
    [SerializeField] private Transform _handWalkTargetL;

    [SerializeField] private Transform _footWalkTargetR;
    [SerializeField] private Transform _footWalkTargetL;
    #endregion

    #region Swing Parameters
    [Header("Swing Constraints")]
    [SerializeField] private TwoBoneIKConstraint _handSwingConstraintR;
    [SerializeField] private TwoBoneIKConstraint _handSwingConstraintL;

    [SerializeField] private TwoBoneIKConstraint _footSwingConstraintR;
    [SerializeField] private TwoBoneIKConstraint _footSwingConstraintL;

    [Header("Swing Targets")]
    [SerializeField] private Transform _handSwingTargetR;
    [SerializeField] private Transform _handSwingTargetL;

    [SerializeField] private Transform _footSwingTargetR;
    [SerializeField] private Transform _footSwingTargetL;
    #endregion
    
    #region Unity Functions
    private void Awake()
    {
        _ref = GetComponentInParent<GibbonRefrences>();
        _rigidbody = _ref.Rigidbody;
    }

    private void Update()
    {
        switch (_ref.Movement.State)
        {
            case MovementState.Ground:
                SetWalkWeights();
                // AccelerationLean();
                // BounceGravity();
                break;

            case MovementState.Swing:
                SwingAnimation();
                break;
            
            case MovementState.Air:
                SetAirWeights();
                AirAnimation();
                break;

            default:
                break;
        }
    }
    #endregion

    #region Weight Helper Functions
    public void SetWalkWeights()
    {
        // Set the active arms Swing IK Constraint weight to 0.
        // Set the active arms walk IK Constraint weight to 1.

        // Set the feet Swing IK Constraint weight to 0.
        // Set the feet Walk IK Constraint weight to 1.
    }

    public void SetSwingWeights(int index)
    {
        // Both arms can be active at the same time so i only need to the arm that is passed.

        // Set the active arms Walk IK constraint weight to 0.
        // Set the active arms Swing IK constraint weight to 1.

        // Set both feet Walk IK constraints weight to 0.
        // Set both feet Swing IK constraints weight to 1.
    }

    public void SetAirWeights()
    {
        // Air weights are the same as walk weights
        SetWalkWeights();
    }
    #endregion

    #region Procedural Walk Animations
    private void RotateTowardsVelocity()
    {
        // This is for third person camera but could be used for swing animation.

        if(_rigidbody.velocity.magnitude < 0.2f)
            return;

        Vector3 projectedVelocity = Vector3.ProjectOnPlane(_rigidbody.velocity.normalized, Vector3.up);
        _ref.PlayerTransform.rotation = Quaternion.Lerp(_ref.PlayerTransform.rotation, Quaternion.LookRotation(projectedVelocity, Vector3.up), Time.deltaTime * 5f);
    }

    private float _tiltAmount = 1.2f;
    private void AccelerationLean()
    {
        // This works but is not what I want
        float dotFoward = Vector3.Dot(_rigidbody.velocity, _ref.PlayerTransform.forward);

        if (dotFoward > 0.25f)
        {
            _ref.ModelTransform.localRotation = Quaternion.Lerp(_ref.ModelTransform.localRotation, Quaternion.Euler(_tiltAmount * dotFoward, 0, 0), Time.deltaTime * 5f);
        }
        else if(dotFoward < -0.25f )
        {
            _ref.ModelTransform.localRotation = Quaternion.Lerp(_ref.ModelTransform.localRotation, Quaternion.Euler(-_tiltAmount * dotFoward, 0, 0), Time.deltaTime * 5f);
        }
        else
            _ref.ModelTransform.localRotation = Quaternion.Lerp(_ref.ModelTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
    }

    [SerializeField] private float _amplitude = 0.1f;
    [SerializeField] private float _frequency = 5f;
    private void BounceGravity()
    {
        _ref.ModelTransform.localPosition = new Vector3(0, Mathf.Sin(Time.time * _frequency) * _amplitude, 0);
    }
    
    private void BalanceArms()
    {
        throw new NotImplementedException();
    }
    
    private void SillyArms()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Procedural Swing Animations
    private void SwingAnimation()
    {
        // Rotate the player so the up direction is always facing the pivot point.
        // Rotate chest so the swinging shoulder is closer to the pivot point.

        // Simulate arm swing
        // Push unused arm away from pivot point.

        // Simulate feet swing
        // Push feet away from pivot point.
        // Feet up and feet down.

        // Lead with unused arm and foot.
    }
    #endregion

    #region Procedural Air Animations
    private void AirAnimation()
    {
        // Predict landing position and rotate body so feet are facing the landing position.
        // Look toward the landing position.
    }
    #endregion
}