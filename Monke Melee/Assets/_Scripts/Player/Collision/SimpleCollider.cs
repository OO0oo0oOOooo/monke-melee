using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCollider : MonoBehaviour
{
    public bool IsGrounded;
    public Vector3 ContactNormal;
    public LayerMask LayerMask;

    [SerializeField] private float _slopeLimit = 45f;
    private void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (contact.normal.y > Mathf.Sin(_slopeLimit * Mathf.Deg2Rad + Mathf.PI / 2f))
            {
                ContactNormal = contact.normal;
                IsGrounded = true;
                return;
            }
        }
    }
}
