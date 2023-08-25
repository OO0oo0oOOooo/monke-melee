using UnityEngine;

public class CrocTargetFinder : MonoBehaviour
{
    public Transform Target;

    void OnTriggerStay(Collider other)
    {
        // Check which trigger is triggering this
        
        // Check line of sight for all players within range
        // Set Destination to closest player.
        // If no line of sight move to last known position.

        if(other.CompareTag("Player"))
        {
            Target = other.gameObject.transform;
        }
    }
}
