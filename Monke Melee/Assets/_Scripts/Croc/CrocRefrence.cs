using Unity.Netcode;
using UnityEngine;

public class CrocRefrence : NetworkBehaviour
{
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public Transform CrocTransform { get; private set; }
    public Transform CrocModel { get; private set; }

    // Scripts
    public RaycastGround RaycastGround { get; private set; }
    public CrocBehaviour CrocBehaviour { get; private set; }
    public CrocTargetFinder CrocTargetFinder { get; private set; }
    public CrocMovement CrocMovement { get; private set; }
    public CrocAttack CrocAttack { get; private set; }
    public CrocHeadController CrocHeadController { get; private set; }

    public void Awake()
    {
        // if(!IsServer) return;

        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();
        CrocTransform = transform;
        CrocModel = Animator.transform;

        RaycastGround = GetComponent<RaycastGround>();
        CrocBehaviour = GetComponent<CrocBehaviour>();
        CrocTargetFinder = GetComponent<CrocTargetFinder>();
        CrocMovement = GetComponent<CrocMovement>();
        CrocAttack = GetComponent<CrocAttack>();
        CrocHeadController = GetComponentInChildren<CrocHeadController>();

        print(CrocHeadController);
    }
}
