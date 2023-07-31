using UnityEngine;

public class SimpleCamera : MonoBehaviour
{
    private Transform _transform;
    private Transform _playerTransform;

    private Camera _cam;
    private CustomInput _customInput;

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
        _transform.rotation = Quaternion.Euler(_customInput.InputRot.x, 0, 0f);
        _playerTransform.rotation = Quaternion.Euler(0f, _customInput.InputRot.y, 0f);
    }
}

