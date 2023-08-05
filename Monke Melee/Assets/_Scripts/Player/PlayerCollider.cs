using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private Player _player;

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _raycastDistance = 1.5f;

    public bool IsGrounded;
    public Vector3 AdverageNormal;
    public Vector3 AdveragePoint;
    public float AdverageDistance;

    void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Update()
    {
        GetNormal();
    }

    [SerializeField] private float radius = 0.5f;
    [SerializeField] private int totalRays = 6;
    [SerializeField] private int numLayers = 2;

    private void GetNormal()
    {
        Vector3 advNormal = Vector3.zero;
        Vector3 advPoint = Vector3.zero;
        float advDistance = 0f;

        int hitCount = 0;

        int remainingRays = totalRays;
        for (int layer = 0; layer < numLayers; layer++)
        {
            int numRaysInLayer;
            if(layer == 0)
                numRaysInLayer = 1;
            else
                numRaysInLayer = Mathf.CeilToInt((float)remainingRays / (numLayers - layer));


            remainingRays -= numRaysInLayer;

            // Calculate the radius of the current layer
            float currentRadius = radius * layer / (numLayers - 1);

            // Perform raycasts from points on the current layer
            for (int i = 0; i < numRaysInLayer; i++)
            {
                float angle = i * 2 * Mathf.PI / numRaysInLayer;
                float x = currentRadius * Mathf.Cos(angle);
                float z = currentRadius * Mathf.Sin(angle);

                Vector3 point = new Vector3(x, 0, z);
                point = _player.PlayerTransform.rotation * point;

                Ray ray = new Ray(_player.PlayerTransform.position + point, point + -_player.PlayerTransform.up);
                Debug.DrawRay(ray.origin, ray.direction * _raycastDistance, Color.red);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f, _layerMask))
                {
                    IsGrounded = true;
                    advNormal += hitInfo.normal;
                    advPoint += hitInfo.point;
                    advDistance += hitInfo.distance;
                    hitCount++;
                }
            }
        }

        if(advNormal == Vector3.zero)
        {
            // Debug.Log("No Ground");
            IsGrounded = false;
            AdverageNormal = transform.up;
            AdveragePoint = Vector3.zero;
            AdverageDistance = 1.2f;
            return;
        }

        AdverageNormal = advNormal.normalized;
        AdveragePoint = advPoint / hitCount;
        AdverageDistance = advDistance / hitCount;
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(_player.PlayerTransform.position, AdverageNormal);
    // }
}
