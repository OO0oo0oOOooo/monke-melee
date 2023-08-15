using UnityEngine;

public class SimpleCamera : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform _playerTransform;

    [SerializeField] private Camera _cam;
    [SerializeField] private CustomInput _customInput;

    private void Awake()
    {
        _transform = transform;
        _playerTransform = transform.parent.transform;

        _cam = GetComponent<Camera>();
        _customInput = GetComponentInParent<CustomInput>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        FirstPersonCamera();
    }

    private void FirstPersonCamera()
    {
        _transform.localRotation = Quaternion.Euler(_customInput.InputRot.x, 0, 0f);
        _playerTransform.rotation = Quaternion.Euler(0f, _customInput.InputRot.y, 0f);
    }
}

