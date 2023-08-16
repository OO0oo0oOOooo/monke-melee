using UnityEngine;
using Unity.Netcode;

public class GibbonRefrences : NetworkBehaviour
{
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
    public ProceduralWalk ProceduralWalk;
    
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

        Camera.gameObject.SetActive(true);
    }
}
