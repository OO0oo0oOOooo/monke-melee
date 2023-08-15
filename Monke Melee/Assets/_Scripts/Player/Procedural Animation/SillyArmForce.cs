using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SillyArmForce : MonoBehaviour
{
    public Rigidbody ArmL;
    public Rigidbody ArmR;

    public float UpForce = 1;
    public float ForwardForce = 1;
    public float AwayForce = 1;

    void FixedUpdate()
    {
        ArmL.AddForce(Vector3.up * UpForce * ArmL.mass);
        ArmL.AddForce(transform.forward * ForwardForce * ArmL.mass);
        ArmL.AddForce((ArmL.position - ArmR.position) * AwayForce * ArmL.mass);

        ArmR.AddForce(Vector3.up * UpForce * ArmR.mass);
        ArmR.AddForce(transform.forward * ForwardForce * ArmR.mass);
        ArmR.AddForce((ArmR.position - ArmL.position) * AwayForce * ArmR.mass);
    }
}
