using UnityEngine;
using Unity.Netcode;

public class GibbonRefrences : NetworkBehaviour
{
    public ulong ClientID { get; private set; }
    public bool IsDead { get; private set;}

    [Header("Components")]
    public Transform PlayerTransform;
    public Transform ModelTransform;
    public Rigidbody Rigidbody;
    public Animator Animator;

    [Header("Scripts")]
    public CustomInput CustomInput;
    public GibbonMovement Movement;
    public GibbonSwing GibbonSwing;
    public SimpleCollider SimpleCollider;
    public ProceduralAnimation ProceduralAnimation;
    public ProceduralWalkEllipse ProceduralWalk;
    
    [Header("Camera")]
    public Camera Camera;
    public Transform CameraTransform;
    public Transform CameraRigTransform;
    public SimpleCamera SimpleCamera;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            Destroy(CustomInput);
            Destroy(Movement);
            Destroy(SimpleCollider);
            Destroy(GibbonSwing);
            // Destroy(ProceduralAnimation);
            // Destroy(ProceduralWalk);
            
            Destroy(Camera.gameObject);
            Destroy(CameraRigTransform.gameObject);
            return;
        }

        ClientID = NetworkObject.OwnerClientId;
        Camera.gameObject.SetActive(true);

        IsDead = false;
        EventManager.Instance.OnPlayerDeath += MarkDead;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;
        
        EventManager.Instance.OnPlayerDeath -= MarkDead;
    }

    public void MarkDead(ulong clientID)
    {
        if(clientID == ClientID)
        {
            IsDead = true;
        }
    }
}
