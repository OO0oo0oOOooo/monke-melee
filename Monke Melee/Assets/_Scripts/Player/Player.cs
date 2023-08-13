using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    // Player
    public Transform PlayerTransform;
    public Transform ModelTransform;
    public Rigidbody Rigidbody;
    public Animator Animator;

    public CustomInput CustomInput;
    public Movement Movement;
    public PlayerCollider PlayerCollider;
    public ProceduralAnimation ProceduralAnimation;

    // Camera
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
            Destroy(PlayerCollider);
            // Destroy(ProceduralAnimation);
            Destroy(PlayerCamera.gameObject);
            Destroy(CameraRigTransform.gameObject);
            return;
        }

        PlayerCamera.gameObject.SetActive(true);
    }
}
