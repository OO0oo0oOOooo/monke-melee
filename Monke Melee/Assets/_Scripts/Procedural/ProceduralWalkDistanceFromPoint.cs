using System.Collections;
using UnityEngine;

public class ProceduralWalkDistanceFromPoint : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private Transform _footTargetR;
    [SerializeField] private Transform _footTargetL;

    [SerializeField] private float _distanceToGround = 0;
    [SerializeField] private Vector3 _raycastOffset = Vector3.zero;
    [SerializeField] private float _footSpacing = 0.25f;
    [SerializeField] private float _wantMoveDistance = 0.5f;

    private Vector3 _footPositionR;
    private Vector3 _footPositionL;

    RaycastHit hitR;
    RaycastHit hitL;

    private bool _canPlaySoundL;
    private bool _canPlaySoundR;

    // If other arm is moving then this arm cant move
    // Cubic lerp between current position and target position

    public bool Moving;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        _footTargetR.position = _footPositionR;
        _footTargetL.position = _footPositionL;

        FootIKSurface();
        FootSounds();
    }

    private void FootIKSurface()
    {
        Ray rayR = new Ray(_transform.position + (_transform.rotation * _raycastOffset) + (_transform.right * _footSpacing), -transform.up);
        Ray rayL = new Ray(_transform.position + (_transform.rotation * _raycastOffset) + (-_transform.right * _footSpacing), -transform.up);

        Debug.DrawRay(rayR.origin, rayR.direction, Color.red);
        Debug.DrawRay(rayL.origin, rayL.direction, Color.red);
        if(Physics.Raycast(rayL, out hitL, _distanceToGround + 1f, _layerMask))
        {
            if(Vector3.Distance(hitL.point, _footTargetL.position) > _wantMoveDistance)
            {
                Vector3 footPosition = hitL.point;
                footPosition.y += _distanceToGround;

                _footPositionL = footPosition;
            }
            
            // _footTargetL.rotation = Quaternion.FromToRotation(transform.up, hitL.normal) * Quaternion.Euler(90,0,0) * transform.rotation;
        }

        if(Physics.Raycast(rayR, out hitR, _distanceToGround + 1f, _layerMask))
        {
            if(Vector3.Distance(hitR.point, _footTargetR.position) > _wantMoveDistance)
            {
                Vector3 footPosition = hitR.point;
                footPosition.y += _distanceToGround;

                _footPositionR = footPosition;
            }

            // _footTargetR.rotation = Quaternion.FromToRotation(transform.up, hitR.normal) * Quaternion.Euler(90,0,0) * transform.rotation;
        }
    }

    // IEnumerator MoveFoot()
    // {
    //     Moving = true;

    //     Vector3 startPoint = _footPositionR;
    //     // Quaternion startRot = transform.rotation;

    //     Vector3 newFootPosition = hitR.point;
    //     newFootPosition.y += _distanceToGround;

    //     // Quaternion endRot = homeTransform.rotation;

    //     // Directional vector from the foot to the home position
    //     Vector3 towardHome = (_footPositionR - newFootPosition);

    //     // Total distnace to overshoot by   
    //     // float overshootDistance = wantStepAtDistance * stepOvershootFraction;
    //     // Vector3 overshootVector = towardHome * overshootDistance;
        
    //     // Since we don't ground the point in this simplified implementation,
    //     // we restrict the overshoot vector to be level with the ground
    //     // by projecting it on the world XZ plane.
    //     // overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

    //     // Apply the overshoot
    //     Vector3 endPoint = hitR.point;

    //     // We want to pass through the center point
    //     Vector3 centerPoint = (startPoint + endPoint) / 2;
    //     // But also lift off, so we move it up by half the step distance (arbitrarily)
    //     centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2f;

    //     float timeElapsed = 0;
    //     do
    //     {
    //         timeElapsed += Time.deltaTime;
    //         float normalizedTime = timeElapsed / moveDuration;

    //         // Quadratic bezier curve
    //         transform.position =
    //         Vector3.Lerp(
    //             Vector3.Lerp(startPoint, centerPoint, normalizedTime),
    //             Vector3.Lerp(centerPoint, endPoint, normalizedTime),
    //             normalizedTime
    //         );

    //         transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

    //         yield return null;
    //     }
    //     while (timeElapsed < moveDuration);

    //     Moving = false;
    // }

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
}
