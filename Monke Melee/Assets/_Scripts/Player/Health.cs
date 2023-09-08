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
        EventManager.Instance.InvokePlayerDeathEvent(_gibbonRefrences.ClientID);
    }
}
