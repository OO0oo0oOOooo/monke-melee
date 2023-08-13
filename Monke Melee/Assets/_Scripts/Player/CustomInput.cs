using System;
using UnityEngine;

public class CustomInput : MonoBehaviour
{
    private Player _player;
    [SerializeField] private string _xAxisInput = "Horizontal";
    [SerializeField] private string _yAxisInput = "Vertical";
    [SerializeField] private string _inputMouseX = "Mouse X";
    [SerializeField] private string _inputMouseY = "Mouse Y";
    [SerializeField] private string _jumpButton = "Jump";

    // Mouse
    [SerializeField] private float _mouseSensitivity = 1f;

    public float _mAxisRawX;
    public float _mAxisRawY;

    public Vector3 InputRot;
    public Vector3 InputRotUnclamped;

    private bool _mouse1Pending = false;
    public bool Mouse1Pending { get => _mouse1Pending; }

    private bool _mouse2Pending = false;
    public bool Mouse2Pending { get => _mouse2Pending; }

    // Jump
    public bool _jumpPending = false;
    public bool JumpPending { get => _jumpPending; }

    // Duck
    private bool _duckPending = false;
    public bool DuckingPending { get => _duckPending; }

    // WASD Input
    private Vector3 _input;
    public Vector3 InputAxis { get => _input; }
    
    private Vector3 _inputRaw;
    public Vector3 InputAxisRaw { get => _inputRaw; }
    
    private Vector3 _inputDirCamera;
    public Vector3 InputDirCamera { get => _inputDirCamera; }

    private Vector3 _inputDirPlayer;
    public Vector3 InputDirPlayer { get => _inputDirPlayer; }

    private Vector3 _inputDirModel;

    public Vector3 InputDirModel { get => _inputDirModel; }


    // Events
    // public static event Action<bool> OnTabPressed;
    // public static event Action<bool> OnEscPressed;

    void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        MouseLook();
        MouseInput();

        MovementInput();
    }

    private void MovementInput()
    {
        float xAxisRaw = Input.GetAxisRaw(_xAxisInput);
        float zAxisRaw = Input.GetAxisRaw(_yAxisInput);
        _inputRaw = new Vector3(xAxisRaw, 0, zAxisRaw).normalized;

        float xAxis = Input.GetAxis(_xAxisInput);
        float zAxis = Input.GetAxis(_yAxisInput);
        _input = new Vector3(xAxis, 0, zAxis);

        _inputDirCamera = _player.CameraTransform.rotation * _inputRaw;
        _inputDirPlayer = _player.PlayerTransform.rotation * _inputRaw;
        _inputDirModel = _player.ModelTransform.rotation * _inputRaw;

        // Jump
        if (Input.GetButtonDown(_jumpButton))
            _jumpPending = true;

        if (Input.GetButtonUp(_jumpButton))
            _jumpPending = false;

        // Duck
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _duckPending = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            _duckPending = false;
    }
    
    private void MouseLook()
    {
        InputRot.y += Input.GetAxisRaw(_inputMouseX) * _mouseSensitivity;
        InputRot.x -= Input.GetAxisRaw(_inputMouseY) * _mouseSensitivity;

        InputRotUnclamped.y += Input.GetAxisRaw(_inputMouseX) * _mouseSensitivity;
        InputRotUnclamped.x -= Input.GetAxisRaw(_inputMouseY) * _mouseSensitivity;

        _mAxisRawX = Input.GetAxisRaw(_inputMouseX) * _mouseSensitivity;
        _mAxisRawY = Input.GetAxisRaw(_inputMouseY) * _mouseSensitivity;

        // clamp
        if (InputRot.x > 90f)
            InputRot.x = 90f;
        if (InputRot.x < -90f)
            InputRot.x = -90f;
    }

    private void MouseInput()
    {
        if(Input.GetMouseButtonDown(0))
            _mouse1Pending = true;

        if(Input.GetMouseButtonUp(0))
            _mouse1Pending = false;


        if(Input.GetMouseButtonDown(1))
            _mouse2Pending = true;

        if(Input.GetMouseButtonUp(1))
            _mouse2Pending = false;
    }
}
