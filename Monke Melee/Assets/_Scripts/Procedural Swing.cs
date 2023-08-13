using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralSwing : MonoBehaviour
{

    [Header("Targets")]
    public Transform HandTarget;
    public Transform ShoulderTarget;
    public Transform HipTarget;

    [Header("Character")]
    public Transform HandIKR;
    public Transform HandIKL;

    public Transform Hand;
    public Transform Chest;
    public Transform Hip;

    public Vector3 _hipPositionOffset;
    public Vector3 _chestPositionOffset;

    public Vector3 _hipRotationOffset;
    public Vector3 _chestRotationOffset;

    // public Rigidbody HandR;
    // public Rigidbody ArmL;

    // public float ForceHand = 5;
    // public float ForceArm = 5;
    // public float AdditionalHandMass = 0;
    // public float AdditionalArmMass = 0;

    void FixedUpdate()
    {
        // HandIKR.position = Vector3.Lerp(HandIKR.position, HandTarget.position, Time.deltaTime * 100);
        // HandIKL.localPosition = -Vector3.Lerp(HandIKL.localPosition, HandTarget.localPosition, Time.deltaTime * 100);

        // HandIKR.localPosition = (HandTarget.position - HandIKR.position);
        // HandIKL.position = -(HandTarget.position - HandIKL.position);

        // HandR.AddForce((HandTarget.position - HandR.position) * ForceHand *  (HandR.mass + AdditionalHandMass));
        // ArmL.AddForce(-(HandTarget.position - ArmL.position) * ForceHand *  (ArmL.mass + AdditionalArmMass));

        Hip.position = Vector3.Lerp(Hip.position, HipTarget.position, Time.deltaTime * 20);
        Hip.rotation = Quaternion.Lerp(Hip.rotation, HipTarget.rotation * Quaternion.Euler(_hipRotationOffset), Time.deltaTime * 20);

        // Chest.position = Vector3.Lerp(Chest.position, ShoulderTarget.position + _chestPositionOffset, Time.deltaTime * 10);
        Chest.rotation = Quaternion.Slerp(Chest.rotation, ShoulderTarget.rotation * Quaternion.Euler(_chestRotationOffset), Time.deltaTime * 20);
    }

    // [SerializeField] private Transform _handTargetR;
    // [SerializeField] private Transform _handTargetL;

    // [Header("")]
    // [SerializeField] private Vector3 _pivotOffset;

    // [SerializeField] private float _swingSpeed = 1f;

    // [SerializeField] private float _ellipseHeight = 0.5f;
    // [SerializeField] private float _ellipseLength = 0.5f;

    // [SerializeField] private float _handSpacing = 0.1f;

    // private float angle = 0.0f;



    // private void FixedUpdate()
    // {
        /* angle -= _swingSpeed * Time.deltaTime;
        float x = _ellipseLength * Mathf.Cos(angle);
        float y = _ellipseHeight * Mathf.Sin(angle);

        _handTargetR.localPosition = _pivotOffset + new Vector3(_handSpacing, y, x);
        _handTargetL.localPosition = _pivotOffset + new Vector3(-_handSpacing, -y, -x); */


    // }
    

    // float angleGizmos = 0;
    // private void OnDrawGizmos()
    // {
    //     Vector3 pivot = transform.position + _pivotOffset;
    //     for (int i = 0; i < 360; i++)
    //     {
    //         float x = _ellipseLength * Mathf.Cos(i * Mathf.PI / 180);
    //         float y = _ellipseHeight * Mathf.Sin(i * Mathf.PI / 180);

    //         Gizmos.DrawSphere(pivot + new Vector3(0, y, x), 0.01f);
    //     }

    //     // angleGizmos -= _stepSpeed * Time.deltaTime;
    //     // float x = _stepLength * Mathf.Cos(angleGizmos);
    //     // float y = _stepHeight * Mathf.Sin(angleGizmos);

    //     // Gizmos.color = Color.white;
    //     // Gizmos.DrawSphere(transform.position + new Vector3(_footSpacing, y, x), 0.1f);
    //     // Gizmos.DrawSphere(transform.position + new Vector3(-_footSpacing, -y, -x), 0.1f);
    // }
}
