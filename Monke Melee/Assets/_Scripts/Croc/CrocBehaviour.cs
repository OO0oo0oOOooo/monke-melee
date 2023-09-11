using System.Collections;
using UnityEngine;

public class CrocBehaviour : MonoBehaviour
{
    // Fields
    private Transform _transform;
    private CrocRefrence _crocRefrence;
    private bool _ableToAttack = true;
    private float _attackCooldown = 3;

    // Enums
    public enum Behaviour
    {
        Idle,   // float or sunbathe
        Wander, // Wander around
        Chase,  // get that ****** and eat him
        Eat     // bring player to water and death roll/eat
    }

    // Properties
    public Vector3 Destination { get; set; }
    public float TargetDistance { get; set; }
    public Vector3 TargetDir { get; set; }

    // Serialized Fields
    [SerializeField] private float _trackRange = 25;
    [SerializeField] private float _jumpRange = 15;
    [SerializeField] private float _biteRange = 2;

    void Awake()
    {
        _transform = transform;
        _crocRefrence = GetComponent<CrocRefrence>();
    }

    void Update()
    {
        switch (GetBehaviourState())
        {
            case Behaviour.Idle:
                Idle();
                break;
            case Behaviour.Wander:
                Wander();
                break;
            case Behaviour.Chase:
                Chase();
                break;
            default:
                break;
        }

        TargetDir = Destination - _transform.position;
    }

    Behaviour GetBehaviourState()
    {
        if(_crocRefrence.CrocTargetFinder.Target == null)
        {
            return Behaviour.Idle;
        }
        else
            return Behaviour.Chase;
    }

    #region Behaviour States
    private void Idle()
    {
        // Do crocadile things like float and sunbathe
        Destination = _transform.position;
    }

    private void Wander()
    {
        // Get random destination every few seconds
        Destination = _transform.position;
    }

    private void Chase()
    {
        TargetDistance = Vector3.Distance(_crocRefrence.CrocTargetFinder.Target.position, _transform.position);

        if(TargetDistance < _trackRange)
        {
            Destination = _crocRefrence.CrocTargetFinder.Target.position;
        }

        
    }
    #endregion

    private IEnumerator AttackCooldown()
    {
        _ableToAttack = false;
        yield return new WaitForSeconds(_attackCooldown);
        _ableToAttack = true;
    }
}
