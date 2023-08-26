using System.Collections;
using UnityEngine;

public class CrocProceduralAnimation : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private LayerMask _layerMask;
    
    [Header("Foot Steppers")]
    [SerializeField] private FootStepper _footStepperFR;
    [SerializeField] private FootStepper _footStepperFL;
    [SerializeField] private FootStepper _footStepperBR;
    [SerializeField] private FootStepper _footStepperBL;

    [Header("Foot Targets")]
    [SerializeField] private Transform _footTargetFR;
    [SerializeField] private Transform _footTargetFL;
    [SerializeField] private Transform _footTargetBR;
    [SerializeField] private Transform _footTargetBL;

    [Header("Foot Raycast")]
    [SerializeField] private Vector3 _frontRaycastOffset = Vector3.zero;
    [SerializeField] private Vector3 _backRaycastOffset = Vector3.zero;
    [SerializeField] private float _raycastDistance = 2;
    [SerializeField] private float _footSpacing = 0.25f;

    [Header("Foot Movement")]
    [SerializeField] private float _distanceToGround = 0;
    [SerializeField] private float _wantMoveDistance = 0.5f;
    [SerializeField] private float _moveDuration = 0.1f;
    [SerializeField] private float _stepOvershootFraction = 1;

    RaycastHit hitFR;
    RaycastHit hitFL;
    RaycastHit hitBR;
    RaycastHit hitBL;

    private bool _canPlaySoundFR;
    private bool _canPlaySoundFL;
    private bool _canPlaySoundBR;
    private bool _canPlaySoundBL;

    void Awake()
    {
        _transform = transform;
        StartCoroutine(LegUpdateCoroutine());
    }

    void Update()
    {
        FootRaycast();
        FootSounds();
    }

    private void FootRaycast()
    {
        Ray rayFR = new Ray(_transform.position + (_transform.rotation * _frontRaycastOffset) + (_transform.right * _footSpacing), -transform.up);
        Ray rayFL = new Ray(_transform.position + (_transform.rotation * _frontRaycastOffset) + (-_transform.right * _footSpacing), -transform.up);
        Ray rayBR = new Ray(_transform.position + (_transform.rotation * _backRaycastOffset) + (_transform.right * _footSpacing), -transform.up);
        Ray rayBL = new Ray(_transform.position + (_transform.rotation * _backRaycastOffset) + (-_transform.right * _footSpacing), -transform.up);

        Physics.Raycast(rayFL, out hitFL, _distanceToGround + _raycastDistance, _layerMask);
        Physics.Raycast(rayFR, out hitFR, _distanceToGround + _raycastDistance, _layerMask);
        Physics.Raycast(rayBL, out hitBL, _distanceToGround + _raycastDistance, _layerMask);
        Physics.Raycast(rayBR, out hitBR, _distanceToGround + _raycastDistance, _layerMask);

        Debug.DrawRay(rayFR.origin, rayFR.direction * _raycastDistance, Color.red);
        Debug.DrawRay(rayFL.origin, rayFL.direction * _raycastDistance, Color.red);
        Debug.DrawRay(rayBR.origin, rayBR.direction * _raycastDistance, Color.red);
        Debug.DrawRay(rayBL.origin, rayBL.direction * _raycastDistance, Color.red);
    }

    IEnumerator LegUpdateCoroutine()
    {
        while(true)
        {
            do
            {
                _footStepperFR.TryStep(hitFR, _wantMoveDistance, _distanceToGround, _moveDuration, _stepOvershootFraction);
                _footStepperBL.TryStep(hitBL, _wantMoveDistance, _distanceToGround, _moveDuration, _stepOvershootFraction);
                yield return null;
            }
            while(_footStepperFR.Moving || _footStepperBL.Moving);
            
            do
            {
                _footStepperFL.TryStep(hitFL, _wantMoveDistance, _distanceToGround, _moveDuration, _stepOvershootFraction);
                _footStepperBR.TryStep(hitBR, _wantMoveDistance, _distanceToGround, _moveDuration, _stepOvershootFraction);
                yield return null;
            }
            while(_footStepperFL.Moving || _footStepperBR.Moving);
        }
    }

    private void FootSounds()
    {
        if(hitFL.distance < _distanceToGround && _canPlaySoundFL)
        {
            _canPlaySoundFL = false;
            PlaySound(hitFL.point);
        }
        else if(hitFL.distance > _distanceToGround)
        {
            _canPlaySoundFL = true;
        }

        if(hitFR.distance < _distanceToGround && _canPlaySoundFR)
        {
            _canPlaySoundFR = false;
            PlaySound(hitFR.point);
        }
        else if(hitFR.distance > _distanceToGround)
        {
            _canPlaySoundFR = true;
        }

        if(hitBL.distance < _distanceToGround && _canPlaySoundBL)
        {
            _canPlaySoundBL = false;
            PlaySound(hitBL.point);
        }
        else if(hitBL.distance > _distanceToGround)
        {
            _canPlaySoundBL = true;
        }

        if(hitBR.distance < _distanceToGround && _canPlaySoundBR)
        {
            _canPlaySoundBR = false;
            PlaySound(hitBR.point);
        }
        else if(hitBR.distance > _distanceToGround)
        {
            _canPlaySoundBR = true;
        }
    }

    private void PlaySound(Vector3 pos)
    {
        AudioSystem.Instance.PlayRandomClipAtPoint((int)GameAudioEnums.Step, pos, 2);
    }
}
