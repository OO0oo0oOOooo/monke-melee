using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform _camTransform;
    [SerializeField] private Transform _camRigTransform;
    [SerializeField] private Transform _playerTransform;

    [SerializeField] private Camera _cam;
    [SerializeField] private CustomInput _customInput;

    private void Awake()
    {
        _camTransform = transform;
        _camRigTransform = transform.parent.transform;
        _playerTransform = transform.parent.parent.transform;

        _cam = GetComponent<Camera>();
        _customInput = _playerTransform.GetComponent<CustomInput>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        ThirdPersonUnlocked();

        // if(_customInput.Attack2Pending)
        //     ThirdPersonUnlocked();
        // else
        //     ThirdPersonLocked();
    }

    private void ThirdPersonUnlocked()
    {
        _camRigTransform.rotation = Quaternion.Euler(_customInput.InputRot.x, _customInput.InputRot.y, 0f);
    }

    private void ThirdPersonLocked()
    {
        _camRigTransform.localRotation = Quaternion.Euler(_customInput.InputRot.x, 0, 0f);
        _playerTransform.localRotation = Quaternion.Euler(0, _customInput.InputRot.y, 0f);
    }
}

