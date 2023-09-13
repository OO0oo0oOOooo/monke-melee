using System.Collections;
using UnityEngine;

public class CrocHeadController : MonoBehaviour
{
    private Transform _transform;
    private CrocRefrence _crocRefrence;

    public Transform HeadBone => _headBone;

    [Range(0,1)][SerializeField] private float _t = 0;

    [SerializeField] private Transform _headBone;
    [SerializeField] private Transform _jawBone;
    [SerializeField] private Vector3 _jawOpenRotation;
    [SerializeField] private Vector3 _jawClosedRotation;
    [SerializeField] private Vector3 _mouthOpenRotation;
    [SerializeField] private Vector3 _mouthClosedRotation;

    [SerializeField] private float headMaxTurnAngle;
    [SerializeField] private float headTrackingSpeed;

    private void Awake()
    {
        _transform = transform;
        _crocRefrence = GetComponentInParent<CrocRefrence>();
    }

    void Update()
    {
        if (_crocRefrence.CrocTargetFinder.Target != null)
        {
            Vector3 direction = (_crocRefrence.CrocTargetFinder.Target.position - transform.position).normalized;

            direction = Vector3.RotateTowards(_transform.forward, direction, Mathf.Deg2Rad * headMaxTurnAngle, 0);

            Quaternion targetRotation = Quaternion.LookRotation(direction, _transform.up) * Quaternion.Euler(90, 0, 0) * Quaternion.Lerp(Quaternion.Euler(_mouthClosedRotation), Quaternion.Euler(_mouthOpenRotation), _t);
            _jawBone.localRotation = Quaternion.Lerp(Quaternion.Euler(_jawClosedRotation), Quaternion.Euler(_jawOpenRotation), _t);

            _headBone.rotation = Quaternion.Slerp( _headBone.rotation, targetRotation, 1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime));
        }
    }

    private IEnumerator OpenCloseMouthCoroutine(float targetTime, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            _t = Mathf.Lerp(_t, targetTime, elapsedTime / duration);
        }

        _t = targetTime;
        
        yield return null;
    }

}
