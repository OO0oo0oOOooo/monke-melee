using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    private GibbonRefrences _gibbonRefrences;

    // Health counted in lives
    
    // When hit by enemy, lose 1 life
    // when 0 lives left, death event

    // Death Event
    // Replace player with ragdoll
    // Freeze Controls
    // Third person camera
    // Game over Text
    // 3 seconds after death player can respawn with any input

    private void Awake()
    {
        _gibbonRefrences = GetComponent<GibbonRefrences>();
    }

    public void TakeDamage(int damage)
    {
        Death();
    }

    [ContextMenu("Death")]
    private void Death()
    {
        EventManager.Instance.OnPlayerDeathEvent(_gibbonRefrences.ClientID);
    }
}
