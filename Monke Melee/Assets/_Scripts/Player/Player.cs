using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    // Player
    public Transform PlayerTransform;
    public Rigidbody Rigidbody;

    public CustomInput CustomInput;
    public Movement Movement;
    public CylinderCollider CylinderCollider;

    // Camera
    public Camera PlayerCamera;
    public Transform CameraTransform;
    public Transform CameraRigTransform;

    public ThirdPersonCamera ThirdPersonCamera;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            Destroy(Movement);
            Destroy(CylinderCollider);
            Destroy(CustomInput);
            Destroy(PlayerCamera.gameObject);
            Destroy(CameraRigTransform.gameObject);
            return;
        }

        PlayerCamera.gameObject.SetActive(true);
    }
}
