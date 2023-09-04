using UnityEngine;

public class CrocBehaviour : MonoBehaviour
{
    private CrocTargetFinder _targetFinder;
    private Transform _transform;

    public Behaviour BehaviourState = Behaviour.Idle;
    public enum Behaviour
    {
        Idle,   // float or sunbathe
        Wander, // Wander around
        Chase,  // get that ****** and eat him
        Eat     // bring player to water and death roll/eat
    }

    [SerializeField] private float _trackRange = 25;

    public Vector3 Destination;
    public float TargetDistance;

    void Awake()
    {
        _transform = transform;
        _targetFinder = GetComponent<CrocTargetFinder>();
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
    }

    Behaviour GetBehaviourState()
    {
        if(_targetFinder.Target == null)
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
        // Get distance to target
        // If target leaves range then set target to null
        // If target is in range then set targetDir

        TargetDistance = Vector3.Distance(_targetFinder.Target.position, _transform.position);

        if(TargetDistance < _trackRange)
        {
            Destination = _targetFinder.Target.position;
        }
    }
    #endregion
}
