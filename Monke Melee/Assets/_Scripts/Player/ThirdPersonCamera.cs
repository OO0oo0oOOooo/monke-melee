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

    private void FixedUpdate()
    {
        ThirdPerson();
    }

    private void ThirdPerson()
    {
        _camRigTransform.localRotation = Quaternion.Euler(_customInput.InputRot.x, 0, 0f);
        _playerTransform.rotation = Quaternion.Euler(0f, _customInput.InputRot.y, 0f);
    }
}

