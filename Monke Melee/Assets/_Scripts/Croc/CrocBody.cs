using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocBody : MonoBehaviour
{
    private List<Vector3> ControlPoints = new List<Vector3>();
    public List<Transform> Segments;

    [SerializeField] private float distanceBetweenControlPoints = 0.8f;

    [SerializeField] private float segmentOffset = 1f;

    // list of segments
    // Segments follow each other like a snake
    // segments raycasy down to get the normal of the ground
    // segments rotate to match the tangent of the surface
    
    // ik chain for the bones in between the segments

    // create a list of positions for the segments to follow
    // the lead segment will place a 

    private void Update()
    {

        ControlPoints.Add(Segments[0].position);

    }
}
