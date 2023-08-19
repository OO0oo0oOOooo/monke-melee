using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    private GibbonRefrences _ref;

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

    private float angleX = 0.0f;
    private float angleY = 0.0f;
    private float angleZ = 0.0f;

    private bool _canPlaySoundL;
    private bool _canPlaySoundR;

    private void Awake()
    {
        _ref = GetComponentInParent<GibbonRefrences>();
    }

    private void Update()
    {
        if(_ref.Movement.State == MovementState.Ground)
            WalkAnimation();
    }

    private void WalkAnimation()
    {
        // Crouch, Gallop

        // TODO:
        // Lerp the ellipse scale velocity
        // The ellipse will have a min and max size.

        FootSteppies2();
        FootIKSurface();
        FootSounds();
    }

    private void FootSteppies()
    {
        float dotX = Vector3.Dot(_ref.Rigidbody.velocity, transform.right); // * transform.right;
        float dotZ = Vector3.Dot(_ref.Rigidbody.velocity, transform.forward); // * transform.forward;

        float magY = _ref.Rigidbody.velocity.magnitude;

        Debug.DrawRay(transform.position, _ref.Rigidbody.velocity.normalized, Color.white);
        Debug.DrawRay(transform.position, transform.forward * 1, Color.blue);
        Debug.DrawRay(transform.position, transform.right * 1, Color.red);

        float stepTime = _stepSpeed * Time.deltaTime;

        angleX -= dotX * stepTime;
        angleY -= magY * stepTime;
        angleZ -= dotZ * stepTime;

        angleX %= 360;
        angleY %= 360;
        angleZ %= 360;

        // Remap dotX 0-1
        // Remap dotY 0-1
        // Remap dotZ 0-1

        dotX = Mathf.Clamp(dotX, -1, 1);
        magY = Mathf.Clamp(magY, -1, 1);
        dotZ = Mathf.Clamp(dotZ, -1, 1);

        float x = (_stepLength * dotX) * Mathf.Cos(angleX);
        float y = (_stepHeight * magY) * Mathf.Sin(angleY);
        float z = (_stepLength * dotZ) * Mathf.Cos(angleZ);

        _footTargetR.localPosition = _ellipseOffset + new Vector3(x, y, z); // + new Vector3(_footSpacing, 0, 0)
        _footTargetL.localPosition = _ellipseOffset + new Vector3(-x, -y, -z); // + new Vector3(-_footSpacing, 0, 0)

        // _footTargetR.position = transform.position + (transform.rotation * new Vector3(x, y, z)); // + new Vector3(_footSpacing, 0, 0)
        // _footTargetL.position = transform.position + (transform.rotation * new Vector3(-x, -y, -z)); // + new Vector3(-_footSpacing, 0, 0)

    }

    private void FootSteppies2()
    {
        Vector3 projectedVel = Vector3.forward;

        if(_ref.Rigidbody.velocity.magnitude > 0.1f)
            projectedVel = Vector3.ProjectOnPlane(_ref.Rigidbody.velocity.normalized, transform.up).normalized;

        Quaternion rot = Quaternion.LookRotation(projectedVel, transform.up);

        Debug.DrawRay(transform.position, _ref.Rigidbody.velocity.normalized, Color.white);
        Debug.DrawRay(transform.position, projectedVel, Color.magenta);

        float dotX = Vector3.Dot(projectedVel, transform.right); // * transform.right;
        float dotZ = Vector3.Dot(projectedVel, transform.forward); // * transform.forward;
        float magY = _ref.Rigidbody.velocity.magnitude;

        float stepTime = _stepSpeed * Time.deltaTime;

        angleY -= magY * stepTime;
        angleY %= 360;
        magY = Mathf.Clamp(magY, -1, 1);

        float y = (_stepHeight * magY) * Mathf.Sin(angleY);
        float z = (_stepLength * magY) * Mathf.Cos(angleY);

        // _footTargetR.position = _ellipseOffset + transform.rotation * (rot * new Vector3(_footSpacing, y, z));
        // _footTargetL.position = _ellipseOffset + transform.rotation * (rot * new Vector3(-_footSpacing, -y, -z));

        _footTargetR.position = transform.position + (rot * new Vector3(_footSpacing, y, z));
        _footTargetL.position = transform.position + (rot * new Vector3(-_footSpacing, -y, -z));

        // _footTargetR.localPosition = (rot * new Vector3(0, y, z));
        // _footTargetL.localPosition = (rot * new Vector3(0, -y, -z));
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
