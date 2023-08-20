using System;
using Unity.Mathematics;
using UnityEngine;
public class ProceduralWalk : MonoBehaviour
{
    private GibbonRefrences _ref;
    private Transform _transform;

    [SerializeField] private Transform _footTargetR;
    [SerializeField] private Transform _footTargetL;

    [SerializeField] private Vector3 _ellipseOffset;
    [SerializeField] private Vector3 _rot;

    [SerializeField] private float _stepSpeed = 1f;
    [SerializeField] private float _stepHeight = 0.5f;
    [SerializeField] private float _stepLength = 0.5f;
    [SerializeField] private float _footSpacing = 0.1f;
    [SerializeField] private float _distanceToGround = 0;

    [SerializeField] private bool _debugEllipse;

    RaycastHit hitR;
    RaycastHit hitL;

    public float angle = 0.0f;

    private bool _canPlaySoundL;
    private bool _canPlaySoundR;

    private void Awake()
    {
        _ref = GetComponentInParent<GibbonRefrences>();
        _transform = transform;
    }

    private void Update()
    {
        if(_ref.Movement.State == MovementState.Ground)
            WalkAnimation();
    }

    private void WalkAnimation()
    {
        FootSteppies();
        FootIKSurface();
        FootSounds();
    }

    private void FootSteppies()
    {
        float stepTime = _stepSpeed * Time.deltaTime;

        float dotX = Vector3.Dot(_ref.Rigidbody.velocity, _transform.right);
        float dotZ = Vector3.Dot(_ref.Rigidbody.velocity, _transform.forward);
        float mag = _ref.Rigidbody.velocity.magnitude;

        angle -= mag * stepTime;

        dotX = Mathf.Clamp(dotX, -1, 1);
        mag = Mathf.Clamp(mag, -1, 1);
        dotZ = Mathf.Clamp(dotZ, -1, 1);

        float x = _stepLength * Mathf.Cos(angle) * dotX;
        float z = _stepLength * Mathf.Cos(angle) * dotZ;
        float y = _stepHeight * mag * Mathf.Sin(angle);

        Vector3 footOffset = new Vector3(Mathf.Abs(dotZ) + 0.001f, 0, Mathf.Abs(dotX) + 0.001f).normalized * _footSpacing;

        _footTargetR.localPosition = _ellipseOffset + (new Vector3(x, y, z) + footOffset);
        _footTargetL.localPosition = _ellipseOffset + (new Vector3(-x, -y, -z) + -footOffset);
    }
    
    private void FootSteppiesTest()
    {
        float stepTime = _stepSpeed * Time.deltaTime;

        float dotX = Vector3.Dot(_ref.Rigidbody.velocity, _transform.right);
        float dotZ = Vector3.Dot(_ref.Rigidbody.velocity, _transform.forward);
        float mag = _ref.Rigidbody.velocity.magnitude;
        float mag2 = Mathf.Sqrt(dotX * dotX + dotZ * dotZ);

        // float weightX = Mathf.Abs(dotX) / (Mathf.Abs(dotX) + Mathf.Abs(dotZ));
        // float weightZ = Mathf.Abs(dotZ) / (Mathf.Abs(dotX) + Mathf.Abs(dotZ));
        // angle -= (weightX * dotX + weightZ * dotZ) * stepTime;

        // float p = 2;
        // float combined = Mathf.Pow(Mathf.Abs(dotX), p) + Mathf.Pow(Mathf.Abs(dotZ), p);
        // combined = Mathf.Sign(dotX + dotZ) * Mathf.Pow(combined, 1 / p);
        // angle -= combined * stepTime;

        mag2 *= Mathf.Sign(dotX + dotZ);
        angle -= mag2 * stepTime;
        // angle -= mag * stepTime;
        // angle -= (dotX+dotZ) * stepTime;

        dotX = Mathf.Clamp(dotX, -1, 1);
        dotZ = Mathf.Clamp(dotZ, -1, 1);
        mag = Mathf.Clamp(mag, -1, 1);

        float x = Mathf.Cos(angle) * Mathf.Abs(dotX) * _stepLength;
        float z = Mathf.Cos(angle) * Mathf.Abs(dotZ) * _stepLength;
        float y = _stepHeight * mag * Mathf.Sin(angle);
        // float y = _stepHeight * Mathf.Sin(angle);

        Vector3 footOffset = new Vector3(Mathf.Abs(dotZ) + 0.001f, 0, Mathf.Abs(dotX) + 0.001f).normalized * _footSpacing;

        _footTargetR.localPosition = _ellipseOffset + (new Vector3(x, y, z) + footOffset);
        _footTargetL.localPosition = _ellipseOffset + (new Vector3(-x, -y, -z) + -footOffset);
    }

    private void FootIKSurface()
    {
        Ray rayR = new Ray(_footTargetR.position + transform.up, -transform.up);
        Ray rayL = new Ray(_footTargetL.position + transform.up, -transform.up);

        if(Physics.Raycast(rayL, out hitL, _distanceToGround + 1f, _ref.SimpleCollider.LayerMask))
        {
            Vector3 footPosition = hitL.point;
            footPosition.y += _distanceToGround;

            _footTargetL.position = footPosition;
            // _footTargetL.rotation = Quaternion.FromToRotation(transform.up, hitL.normal) * Quaternion.Euler(90,0,0) * transform.rotation;
        }

        if(Physics.Raycast(rayR, out hitR, _distanceToGround + 1f, _ref.SimpleCollider.LayerMask))
        {
            Vector3 footPosition = hitR.point;
            footPosition.y += _distanceToGround;

            _footTargetR.position = footPosition;
            // _footTargetR.rotation = Quaternion.FromToRotation(transform.up, hitR.normal) * Quaternion.Euler(90,0,0) * transform.rotation;
        }
    }

    private void FootSounds()
    {
        if(hitL.distance < _distanceToGround && _canPlaySoundL)
        {
            _canPlaySoundL = false;
            PlaySound(hitL.point);
        }
        else if(hitL.distance > _distanceToGround)
        {
            _canPlaySoundL = true;
        }

        if(hitR.distance < _distanceToGround && _canPlaySoundR)
        {
            _canPlaySoundR = false;
            PlaySound(hitR.point);
        }
        else if(hitR.distance > _distanceToGround)
        {
            _canPlaySoundR = true;
        }
    }

    private void PlaySound(Vector3 pos)
    {
        AudioSystem.Instance.PlayRandomClipAtPoint((int)GameAudioEnums.Step, pos, 2);
    }

    private void OnDrawGizmos()
    {
        if(!_debugEllipse) return;

        Vector3 pivot = transform.position + (transform.rotation * _ellipseOffset);
        for (int i = 0; i < 360; i++)
        {
            float x = _stepLength * Mathf.Cos(i * Mathf.PI / 180);
            float y = _stepHeight * Mathf.Sin(i * Mathf.PI / 180);

            Gizmos.DrawSphere(pivot + new Vector3(0, y, x), 0.01f);
        }
    }
}
