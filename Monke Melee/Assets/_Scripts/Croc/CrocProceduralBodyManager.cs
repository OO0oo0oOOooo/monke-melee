using System.Collections.Generic;
using UnityEngine;

public class CrocProceduralBodyManager : MonoBehaviour
{
    public List<Transform> spineBones;
    [SerializeField] private LayerMask _layerMask;

    // Raycast to the local z direction from each bone
    // rotate bone to match normal of hit

    // private void Start()
    // {
    //     spineBones = new List<Transform>();
    //     Transform[] children = GetComponentsInChildren<Transform>();
    //     foreach (Transform child in children)
    //     {
    //         if (child.name.Contains("Spine"))
    //         {
    //             spineBones.Add(child);
    //         }
    //     }
    // }

    // Clamp bones to a certain angle
    // Interpolate Rotation
    // Lerp to default rotation when hit is null

    private void Update()
    {
        foreach (Transform bone in spineBones)
        {
            RaycastHit hit;
            if (Physics.Raycast(bone.position, bone.forward, out hit, 1f, _layerMask))
            {
                bone.rotation = Quaternion.LookRotation(hit.normal, bone.up);
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Transform bone in spineBones)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(bone.position, bone.forward);
        }
    }
}
