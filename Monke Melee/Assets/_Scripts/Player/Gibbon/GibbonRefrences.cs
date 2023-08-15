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
    public SimpleCollider SimpleCollider;
    public ProceduralAnimation ProceduralAnimation;

    [Header("Camera")]
    public Camera PlayerCamera;
    public Transform CameraTransform;
    public Transform CameraRigTransform;
    public ThirdPersonCamera ThirdPersonCamera;


    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            Destroy(CustomInput);
            Destroy(Movement);
            Destroy(SimpleCollider);
            // Destroy(ProceduralAnimation);
            Destroy(PlayerCamera.gameObject);
            Destroy(CameraRigTransform.gameObject);
            return;
        }

        PlayerCamera.gameObject.SetActive(true);
    }
}
