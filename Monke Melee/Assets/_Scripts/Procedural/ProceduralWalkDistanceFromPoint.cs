using System.Collections;
using UnityEngine;

public class ProceduralWalkDistanceFromPoint : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private LayerMask _layerMask;
    
    [SerializeField] private FootStepper _footStepperR;
    [SerializeField] private FootStepper _footStepperL;

    [SerializeField] private Transform _footTargetR;
    [SerializeField] private Transform _footTargetL;

    [SerializeField] private Vector3 _raycastOffset = Vector3.zero;
    [SerializeField] private float _footSpacing = 0.25f;

    [SerializeField] private float _distanceToGround = 0;
    [SerializeField] private float _wantMoveDistance = 0.5f;
    [SerializeField] private float _moveDuration = 0.1f;
    [SerializeField] private float _stepOvershootFraction = 1;

    RaycastHit hitR;
    RaycastHit hitL;

    private bool _canPlaySoundL;
    private bool _canPlaySoundR;

    private void Awake()
    {
        _transform = transform;
    }

    private void Update()
    {
        FootIKSurface();
        FootSounds();

        _footTargetR.position = _footStepperR.FootPosition;
        _footTargetL.position = _footStepperL.FootPosition;
    }

    // private void FootIKSurface()
    // {
    //     Ray rayR = new Ray(_transform.position + (_transform.rotation * _raycastOffset) + (_transform.right * _footSpacing), -transform.up);
    //     Ray rayL = new Ray(_transform.position + (_transform.rotation * _raycastOffset) + (-_transform.right * _footSpacing), -transform.up);

    //     Debug.DrawRay(rayR.origin, rayR.direction, Color.red);
    //     Debug.DrawRay(rayL.origin, rayL.direction, Color.red);
    //     if(Physics.Raycast(rayL, out hitL, _distanceToGround + 1f, _layerMask))
    //     {
    //         if(Vector3.Distance(hitL.point, _footTargetL.position) > _wantMoveDistance)
    //         {
    //             Vector3 footPosition = hitL.point;
    //             footPosition.y += _distanceToGround;

    //             _footPositionL = footPosition;
    //         }
            
    //         // _footTargetL.rotation = Quaternion.FromToRotation(transform.up, hitL.normal) * Quaternion.Euler(90,0,0) * transform.rotation;
    //     }

    //     if(Physics.Raycast(rayR, out hitR, _distanceToGround + 1f, _layerMask))
    //     {
    //         if(Vector3.Distance(hitR.point, _footTargetR.position) > _wantMoveDistance)
    //         {
    //             Vector3 footPosition = hitR.point;
    //             footPosition.y += _distanceToGround;

    //             _footPositionR = footPosition;
    //         }

    //         // _footTargetR.rotation = Quaternion.FromToRotation(transform.up, hitR.normal) * Quaternion.Euler(90,0,0) * transform.rotation;
    //     }
    // }

    private void FootIKSurface()
    {
        Ray rayR = new Ray(_transform.position + (_transform.rotation * _raycastOffset) + (_transform.right * _footSpacing), -transform.up);
        Ray rayL = new Ray(_transform.position + (_transform.rotation * _raycastOffset) + (-_transform.right * _footSpacing), -transform.up);

        Debug.DrawRay(rayR.origin, rayR.direction, Color.red);
        Debug.DrawRay(rayL.origin, rayL.direction, Color.red);
        if(Physics.Raycast(rayL, out hitL, _distanceToGround + 1f, _layerMask))
        {
            if(Vector3.Distance(hitL.point, _footTargetL.position) > _wantMoveDistance && !_footStepperR.Moving)
            {
                StartCoroutine(_footStepperL.Step(hitL, _wantMoveDistance, _distanceToGround, _moveDuration, _stepOvershootFraction));
            }
        }

        if(Physics.Raycast(rayR, out hitR, _distanceToGround + 1f, _layerMask))
        {
            if(Vector3.Distance(hitR.point, _footTargetR.position) > _wantMoveDistance && !_footStepperL.Moving)
            {
                StartCoroutine(_footStepperR.Step(hitR, _wantMoveDistance, _distanceToGround, _moveDuration, _stepOvershootFraction));
            }
        }
    }

    // IEnumerator MoveFoot(Vector3 hitPoint)
    // {
    //     Moving = true;

    //     Vector3 startPoint = _footPositionR;
    //     // Quaternion startRot = transform.rotation;

    //     Vector3 newFootPosition = hitPoint;
    //     newFootPosition.y += _distanceToGround;

    //     Vector3 towardHome = (_footPositionR - newFootPosition);

    //     float overshootDistance = _wantMoveDistance * _stepOvershootFraction;
    //     Vector3 overshootVector = towardHome * overshootDistance;
        
    //     overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

    //     Vector3 endPoint = hitPoint + overshootVector;
    //     // Quaternion endRot = homeTransform.rotation;

    //     // We want to pass through the center point
    //     Vector3 centerPoint = (startPoint + endPoint) / 2;
    //     // But also lift off, so we move it up by half the step distance (arbitrarily)
    //     centerPoint += _transform.up * Vector3.Distance(startPoint, endPoint) / 2f;

    //     float timeElapsed = 0;
    //     do
    //     {
    //         timeElapsed += Time.deltaTime;
    //         float normalizedTime = timeElapsed / _moveDuration;

    //         // Quadratic bezier curve
    //         _footPositionR =
    //         Vector3.Lerp(
    //             Vector3.Lerp(startPoint, centerPoint, normalizedTime),
    //             Vector3.Lerp(centerPoint, endPoint, normalizedTime),
    //             normalizedTime
    //         );

    //         // transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

    //         yield return null;
    //     }
    //     while (timeElapsed < _moveDuration);

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
