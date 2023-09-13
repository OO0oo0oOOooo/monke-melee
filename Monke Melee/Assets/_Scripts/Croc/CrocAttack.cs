using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CrocAttack : NetworkBehaviour
{
    // Fields
    private CrocRefrence _crocRefrence;
    private bool _ableToBite = true;
    private bool _duringTackle = false;

    // Properties
    private Vector3 BiteHitboxPosition => _crocRefrence.CrocHeadController.HeadBone.position + (_crocRefrence.CrocHeadController.HeadBone.rotation * _biteHitboxOffset);

    // Serialized Fields
    [SerializeField] private float _attackCooldown = 3;
    [SerializeField] private Vector3 _biteHitboxOffset;
    [SerializeField] private LayerMask _hitableLayers;
    [SerializeField] private float _biteHitboxRadius = 1;
    [SerializeField] private float _jumpHeight = 50;

    private void Awake()
    {
        _crocRefrence = GetComponent<CrocRefrence>();
    }

    private void Update()
    {
        if(!IsServer) return;

        BiteTrigger();

        // Open mouth % based on distance
    }


    private void BiteTrigger()
    {
        Collider[] hitColliders = Physics.OverlapSphere(BiteHitboxPosition, _biteHitboxRadius, _hitableLayers);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Player"))
            {
                // Close Mouth
                // Play sound effect
                // Deathevent replaces player with ragdoll and third person camera
                
                AttackHit(hitCollider.gameObject);
                // StartCoroutine(AttackCooldown());
            }
        }
    }

    // public void Tackle()
    // {
    //     Debug.Log("Tackle");
    //     _duringTackle = true;

    //     _rb.AddForce(_jumpHeight * _rb.mass * _behaviour.TargetDir);
    //     StartCoroutine(TackleCoroutine());
    // }

    // private IEnumerator TackleCoroutine()
    // {
    //     while(_duringTackle)
    //     {
    //         if(_groundCheck.IsGrounded)
    //         {
    //             yield return new WaitForSeconds(0.1f);
    //             _duringTackle = false;
    //         }
    //     }

    //     yield return null;
    // }

    private void AttackHit(GameObject player)
    {
        if(!IsServer) return;

        GibbonRefrences gibbonRefrences = player.GetComponent<GibbonRefrences>();

        if(gibbonRefrences.IsDead) return;
        Debug.Log("Attack Hit");
        
        EventManager.Instance.InvokePlayerDeathEvent(gibbonRefrences.ClientID);
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(BiteHitboxPosition, _biteHitboxRadius);
    }
}
