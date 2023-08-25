using System;
using UnityEngine;

public class RaycastGround : MonoBehaviour
{
    private Transform _transform;

    public bool IsGrounded;
    public Vector3 AdvNormal;
    public Vector3 AdvPoint;
    
    [SerializeField] private float _slopeLimit = 45f;

    public LayerMask LayerMask;

    [SerializeField] private RaySettings[] _ray;

    public bool DebugGizmos = false;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        Raycast();

        if(Vector3.Angle(AdvNormal, Vector3.up) > _slopeLimit)
            IsGrounded = false;
    }

    private void Raycast()
    {
        Vector3 advNormal = Vector3.zero;
        Vector3 advPoint = Vector3.zero;
        int hitCount = 0;

        for (int i = 0; i < _ray.Length; i++)
        {
            Vector3 origin = _transform.position + (_transform.rotation * _ray[i].Origin);
            Vector3 dir = _transform.rotation * _ray[i].Direction;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, _ray[i].Distance, LayerMask))
            {
                advNormal += hit.normal;
                IsGrounded = true;
                hitCount++;
            }
        }

        if(hitCount == 0)
        {
            IsGrounded = false;
            AdvNormal = transform.up;
            AdvPoint = Vector3.zero;
            return;
        }

        AdvNormal = advNormal.normalized;
        AdvPoint = advPoint / hitCount;
    }

    private void OnDrawGizmos()
    {
        if(!DebugGizmos) return;
        if(_ray.Length == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < _ray.Length; i++)
        {
            Vector3 origin = transform.position + (transform.rotation * _ray[i].Origin);
            Vector3 dir = transform.rotation * _ray[i].Direction;
            Gizmos.DrawRay(origin, dir * _ray[i].Distance);
        }
    }
}

[Serializable]
public struct RaySettings
{
    public Vector3 Origin;
    public Vector3 Direction;
    public float Distance;
}
