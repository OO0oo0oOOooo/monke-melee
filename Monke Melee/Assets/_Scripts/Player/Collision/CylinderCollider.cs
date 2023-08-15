using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CylinderCollider : MonoBehaviour
{
    [SerializeField] Player _player;
    public GameObject Cylinder;


    [Header("Collision Parameters")]
    [SerializeField] private float _slopeLimit = 45;
    public Vector3 ContactNormal;
    public bool OnGround = false;


    [Header("SphereCast Parameters")]
    [SerializeField] private LayerMask _layerMask;
    public float CapsuleHalfHeight;


    [Header("SphereCast Feet Parameters")]
    [SerializeField] private float _spherecastFeetStart = 0f;
    [SerializeField] private float _spherecastFeetRadius = 0.32f;
    private RaycastHit _feetHit;
    public RaycastHit FeetHit { get => _feetHit; }
    public bool SphereCastGrounded {get; private set; }


    [Header("SphereCast Head Parameters")]
    [SerializeField] private float _spherecastHeadStart = 0f;
    [SerializeField] private float _spherecastHeadRadius = 0.3f;
    private RaycastHit _headHit;
    public RaycastHit HeadHit { get => _headHit; }


    [Header("Steps")]
    public float _maxStepHeight = 0.4f;
    public float _stepSearchOvershoot = 0.01f;
    private List<ContactPoint> _contactPoints = new List<ContactPoint>();
    private Vector3 _lastVelocity;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        CapsuleHalfHeight = Cylinder.transform.localScale.y;
    }

    private void FixedUpdate()
    {
        SphereCastFeet();
        // Step();
    }

    public void SetHeight(float value)
    {
        Cylinder.transform.localScale = new Vector3(1, value, 1);
        CapsuleHalfHeight = value;
    }

    private void SphereCastFeet()
    {
        if (Physics.SphereCast(Cylinder.transform.position + (transform.up * _spherecastFeetStart), _spherecastFeetRadius, -transform.up, out _feetHit, CapsuleHalfHeight, _layerMask))
            SphereCastGrounded = true;
        else
            SphereCastGrounded = false;
    }

    public bool SphereCastHead()
    {
        if (Physics.SphereCast(Cylinder.transform.position + (transform.up * _spherecastHeadStart), _spherecastHeadRadius, transform.up, out _headHit, CapsuleHalfHeight, _layerMask))
            return true;
        else
            return false;
    }

    // void OnCollisionEnter(Collision col)
    // {
    //     _contactPoints.AddRange(col.contacts);
    // }
 
    private void OnCollisionStay(Collision other)
    {
        _contactPoints.AddRange(other.contacts);

        foreach (ContactPoint contact in other.contacts)
        {
            if (contact.normal.y > Mathf.Sin(_slopeLimit * Mathf.Deg2Rad + Mathf.PI / 2f))
            {
                ContactNormal = contact.normal;
                OnGround = true;
                return;
            }
        }
    }


    // #region Steps
    // private void Step()
    // {
    //     Vector3 velocity = _player.Rigidbody.velocity;

    //     bool grounded = FindGround(out ContactPoint groundCP, _contactPoints);

    //     Vector3 stepUpOffset = default;
    //     bool stepUp = false;
    //     if(grounded)
    //         stepUp = FindStep(out stepUpOffset, _contactPoints, groundCP, velocity);
        
    //     if(stepUp)
    //     {
    //         _player.Rigidbody.position += stepUpOffset;
    //         _player.Rigidbody.velocity = _lastVelocity;
    //     }
        
    //     _contactPoints.Clear();
    //     _lastVelocity = velocity;
    // }

    // bool FindGround(out ContactPoint groundCP, List<ContactPoint> contactPoints)
    // {
    //     groundCP = default;
    //     bool found = false;
    //     foreach(ContactPoint point in contactPoints)
    //     {   
    //         if(point.normal.y > 0.0001f && (found == false || point.normal.y > groundCP.normal.y))
    //         {
    //             groundCP = point;
    //             found = true;
    //         }
    //     }
        
    //     return found;
    // }
    
    // bool FindStep(out Vector3 stepUpOffset, List<ContactPoint> contactPoints, ContactPoint groundCP, Vector3 currVelocity)
    // {
    //     stepUpOffset = default;
        
    //     Vector2 velocityXZ = new Vector2(currVelocity.x, currVelocity.z);
    //     if(velocityXZ.sqrMagnitude < 0.0001f)
    //         return false;
        
    //     foreach(ContactPoint cp in contactPoints)
    //     {
    //         bool test = ResolveStepUp(out stepUpOffset, cp, groundCP);
    //         if(test)
    //             return test;
    //     }
    //     return false;
    // }
    
    // bool ResolveStepUp(out Vector3 stepUpOffset, ContactPoint stepTestCP, ContactPoint groundCP)
    // {
    //     stepUpOffset = default;
    //     Collider stepCol = stepTestCP.otherCollider;
        
    //     if(Mathf.Abs(stepTestCP.normal.y) >= 0.01f)
    //         return false;
        
    //     if( !(stepTestCP.point.y - groundCP.point.y < _maxStepHeight) )
    //         return false;

    //     float stepHeight = groundCP.point.y + _maxStepHeight + 0.0001f;
    //     Vector3 stepTestInvDir = new Vector3(-stepTestCP.normal.x, 0, -stepTestCP.normal.z).normalized;

    //     Vector3 origin = new Vector3(stepTestCP.point.x, stepHeight, stepTestCP.point.z) + (stepTestInvDir * _stepSearchOvershoot);
    //     Vector3 direction = Vector3.down;

    //     if(!stepCol.Raycast(new Ray(origin, direction), out RaycastHit hitInfo, _maxStepHeight))
    //         return false;

    //     Vector3 stepUpPoint = new Vector3(stepTestCP.point.x, hitInfo.point.y+0.0001f, stepTestCP.point.z) + (stepTestInvDir * _stepSearchOvershoot);
    //     Vector3 stepUpPointOffset = stepUpPoint - new Vector3(stepTestCP.point.x, groundCP.point.y, stepTestCP.point.z);
        
    //     stepUpOffset = stepUpPointOffset;
    //     return true;
    // }
    // #endregion

    [SerializeField] private bool _debugGroundNormal = false;
    [SerializeField] private bool _debugSphereCast = false;
    
    private void OnDrawGizmos()
    {
        if(_debugSphereCast)
        {
            // SPHERECAST VISUAL
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(Cylinder.transform.position + (transform.up * _spherecastFeetStart) + (-transform.up * CapsuleHalfHeight), _spherecastFeetRadius);
            Gizmos.DrawWireSphere(Cylinder.transform.position + (transform.up * _spherecastHeadStart) + (transform.up * CapsuleHalfHeight), _spherecastHeadRadius);
        }

        if(_debugGroundNormal)
        {
            // NORMAL VISUAL
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_feetHit.point, 0.1f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_feetHit.point, _feetHit.point + _feetHit.normal * 2);
        }
    }

    public bool RaycastWallForward(float distance)
    {
        Debug.DrawRay(_player.PlayerTransform.position, _player.PlayerTransform.forward * distance, Color.red);

        if(Physics.Raycast(_player.PlayerTransform.position, _player.PlayerTransform.forward, out FrontWallHit, distance, _layerMask))
            return true;
        else
            return false;
    }


    [Header("Raycast Wall")]
    [SerializeField] private Vector3 _rayUpOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _rayDownOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _rayRightOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 _rayLeftOffset = new Vector3(0, 0, 0);

    [SerializeField] private Vector3 _rayUpDir = new Vector3(0, 1, 0.5f);
    [SerializeField] private Vector3 _rayDownDir = new Vector3(0, -1, 0.5f);
    [SerializeField] private Vector3 _rayRightDir = new Vector3(1, 0, 0.5f);
    [SerializeField] private Vector3 _rayLeftDir = new Vector3(-1, 0, 0.5f);

    public RaycastHit FrontWallHit;
    public RaycastHit UpWallHit;
    public RaycastHit DownWallHit;
    public RaycastHit RightWallHit;
    public RaycastHit LeftWallHit;
    public void RaycastWall(float distance)
    {
        Vector3 pos = _player.PlayerTransform.position;

        // Could make an array of all climbing raycasts and loop through them

        // Raycast forward, if hit, set FrontWallHit, if no hit set FrontWallHit to null;
        if (Physics.Raycast(pos, _player.PlayerTransform.forward, out RaycastHit frontHit, distance, _layerMask))
            FrontWallHit = frontHit;
        else
            FrontWallHit = default;

        // Raycast up, if hit, set UpWallHit, if no hit set UpWallHit to null;
        if (Physics.Raycast(pos + _rayUpOffset, _player.PlayerTransform.rotation * _rayUpDir, out RaycastHit upHit, distance, _layerMask))
            UpWallHit = upHit;
        else
            UpWallHit = default;

        // Raycast down, if hit, set DownWallHit, if no hit set DownWallHit to null;
        if (Physics.Raycast(pos + _rayDownOffset, _player.PlayerTransform.rotation * _rayDownDir, out RaycastHit downHit, distance, _layerMask))
            DownWallHit = downHit;
        else
            DownWallHit = default;

        // Raycast right, if hit, set RightWallHit, if no hit set RightWallHit to null;
        if (Physics.Raycast(pos + _rayRightOffset, _player.PlayerTransform.rotation * _rayRightDir, out RaycastHit rightHit, distance, _layerMask))
            RightWallHit = rightHit;
        else
            RightWallHit = default;

        // Raycast left, if hit, set LeftWallHit, if no hit set LeftWallHit to null;
        if (Physics.Raycast(pos + _rayLeftOffset, _player.PlayerTransform.rotation * _rayLeftDir, out RaycastHit leftHit, distance, _layerMask))
            LeftWallHit = leftHit;
        else
            LeftWallHit = default;

        // Draw Debug Rays for all
        Debug.DrawRay(pos, _player.PlayerTransform.forward * distance, Color.white);
        Debug.DrawRay(pos + _rayUpOffset, _player.PlayerTransform.rotation * (_rayUpDir * distance), Color.white);
        Debug.DrawRay(pos + _rayDownOffset, _player.PlayerTransform.rotation * (_rayDownDir * distance), Color.white);
        Debug.DrawRay(pos + _rayRightOffset, _player.PlayerTransform.rotation * (_rayRightDir * distance), Color.white);
        Debug.DrawRay(pos + _rayLeftOffset, _player.PlayerTransform.rotation * (_rayLeftDir * distance), Color.white);
    }

}