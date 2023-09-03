using System.Collections.Generic;
using UnityEngine;

public class CrocTargetFinder : MonoBehaviour
{
    public Transform Target;
    public List<Transform> TargetsInRange = new List<Transform>();

    void Update()
    {
        FindClosestTarget();
    }

    void FindClosestTarget()
    {
        if(TargetsInRange.Count == 0)
        {
            Target = null;
            return;
        }

        int closestTargetIndex = 0;
        float closestTargetDistance = Mathf.Infinity;

        for (int i = 0; i < TargetsInRange.Count; i++)
        {
            float dist = Vector3.Distance(TargetsInRange[i].position, transform.position);
            if(dist < closestTargetDistance)
            {
                closestTargetIndex = i;
                closestTargetDistance = dist;
            }
        }

        Target = TargetsInRange[closestTargetIndex];
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            TargetsInRange.Add(other.gameObject.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            TargetsInRange.Remove(other.gameObject.transform);
        }
    }
}
