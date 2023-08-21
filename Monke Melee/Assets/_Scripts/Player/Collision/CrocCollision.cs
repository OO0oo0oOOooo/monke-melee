using UnityEngine;

public class CrocCollision : MonoBehaviour
{
    private Transform _transform;

    public bool IsGrounded;
    public Vector3 AdvNormal;
    public Vector3 AdvPoint;
    
    [SerializeField] private float _slopeLimit = 45f;

    public LayerMask LayerMask;
    [SerializeField] private Vector3[] _raycastOrigins;
    [SerializeField] private float _raycastDistance = 1f;

    public bool DebugGizmos = false;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        RaycastGround();

        if(Vector3.Angle(AdvNormal, Vector3.up) > _slopeLimit)
            IsGrounded = false;
    }

    private void RaycastGround()
    {
        Vector3 advNormal = Vector3.zero;
        Vector3 advPoint = Vector3.zero;
        int hitCount = 0;

        for (int i = 0; i < _raycastOrigins.Length; i++)
        {
            Vector3 dir = -_transform.up;
            Vector3 origin = _transform.position + (_transform.rotation * _raycastOrigins[i]);

            if (Physics.Raycast(origin, dir, out RaycastHit hit, _raycastDistance, LayerMask))
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

        Gizmos.color = Color.red;
        for (int i = 0; i < _raycastOrigins.Length; i++)
        {
            Vector3 dir = -transform.up;
            Vector3 origin = transform.position + (transform.rotation * _raycastOrigins[i]);
            Gizmos.DrawRay(origin, dir * _raycastDistance);
        }
    }
}
