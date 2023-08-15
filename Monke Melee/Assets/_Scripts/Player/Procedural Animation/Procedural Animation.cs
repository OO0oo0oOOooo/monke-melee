using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ProceduralAnimation : MonoBehaviour
{
    private GibbonRefrences _ref;
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private Transform _headTarget;
    // [SerializeField] private Transform _hipTarget;

    // [SerializeField] private Transform _handTargetR;
    // [SerializeField] private Transform _handTargetL;

    // [SerializeField] private Transform _footTargetR;
    // [SerializeField] private Transform _footTargetL;

    // [Header("")]
    // [SerializeField] private LayerMask _layerMask;
    // [SerializeField] private Vector3 _pivotOffset;


    // private float angle = 0.0f;
    // [SerializeField] private float _stepSpeed = 1f;

    // [SerializeField] private float _stepHeight = 0.5f;
    // [SerializeField] private float _stepLength = 0.5f;

    // [SerializeField] private float _footSpacing = 0.1f;


    // [SerializeField] private float _distanceToGround = 0;

    private void Awake()
    {
        _ref = GetComponentInParent<GibbonRefrences>();
        _rigidbody = _ref.Rigidbody;
    }

    private void Update()
    {
        // RotateTowardsVelocity();
        // AccelerationLean();
        // BounceGravity();

        // LookAtTarget();

        
        // BalanceArms();
        // TwoPartSpringPendulum();
    }

    private void RotateTowardsVelocity()
    {
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

    private void TwoPartSpringPendulum()
    {
        throw new NotImplementedException();
    }

    private void LookAtTarget()
    {
        // Look at next swing point
        // Look at Movement Direction
        // Look at Landing point

        // I could use multiple targets and switch between whatever one i want to look at.


        // Set Constraint Weights

        // _headTarget.
    }
}
