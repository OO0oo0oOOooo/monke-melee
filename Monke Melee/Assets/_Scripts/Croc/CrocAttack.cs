using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CrocAttack : NetworkBehaviour
{
    private Rigidbody _rb;
    private CrocBehaviour _behaviour;
    private RaycastGround _groundCheck;

    [SerializeField] private float _attackCooldown = 3;
    private bool _ableToBite = true;

    public bool DuringTackle = false;

    [SerializeField] private LayerMask _hitableLayers;
    [SerializeField] private Transform _mouthBone;
    [SerializeField] private Vector3 _mouthOffset;
    [SerializeField] private float _biteHitboxRadius = 1;

    [SerializeField] private float _jumpHeight = 50;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _behaviour = GetComponent<CrocBehaviour>();
        _groundCheck = GetComponent<RaycastGround>();
    }

    private void Update()
    {
        if(!IsServer) return;

        // if(_ableToBite)
        BiteTrigger();
    }

    private void BiteTrigger()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_mouthBone.position + (_mouthBone.rotation * _mouthOffset), _biteHitboxRadius, _hitableLayers);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Player"))
            {
                // Play Bite animation
                AttackHit(hitCollider.gameObject);
                // StartCoroutine(AttackCooldown());
            }
        }
    }

    public void Tackle()
    {
        Debug.Log("Tackle");
        DuringTackle = true;

        _rb.AddForce(_jumpHeight * _rb.mass * _behaviour.TargetDir);
        StartCoroutine(TackleCoroutine());
    }

    private IEnumerator TackleCoroutine()
    {
        while(DuringTackle)
        {
            BiteHitbox();

            if(_groundCheck.IsGrounded)
            {
                yield return new WaitForSeconds(0.1f);
                DuringTackle = false;
            }
        }

        yield return null;
    }

    private void BiteHitbox()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_mouthBone.position + (_mouthBone.rotation * _mouthOffset), _biteHitboxRadius, _hitableLayers);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.CompareTag("Player"))
            {
                AttackHit(hitCollider.gameObject);
                DuringTackle = false;
            }
        }
    }

    private void AttackHit(GameObject player)
    {
        if(!IsServer) return;

        GibbonRefrences gibbonRefrences = player.GetComponent<GibbonRefrences>();

        if(gibbonRefrences.IsDead) return;
        Debug.Log("Attack Hit");
        
        EventManager.Instance.InvokePlayerDeathEvent(gibbonRefrences.ClientID);
    }

    private IEnumerator AttackCooldown()
    {
        _ableToBite = false;
        yield return new WaitForSeconds(_attackCooldown);
        _ableToBite = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_mouthBone.position + (_mouthBone.rotation * _mouthOffset), _biteHitboxRadius);
    }
}
