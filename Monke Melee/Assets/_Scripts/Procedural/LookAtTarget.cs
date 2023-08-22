using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    private Transform _transform;

    [SerializeField] private CrocTargetFinder _targetFinder;
    [SerializeField] private Transform _headBone;

    [SerializeField] private float headMaxTurnAngle;
    [SerializeField] private float headTrackingSpeed;

    private void Awake()
    {
        _transform = transform;
        _targetFinder = GetComponentInParent<CrocTargetFinder>();
    }

    void Update()
    {
        if (_targetFinder.Target != null)
        {
            Vector3 direction = (_targetFinder.Target.position - transform.position).normalized;

            // Clamp direction -headMaxTurnAngle and +headMaxTurnAngle
            direction = Vector3.RotateTowards(_transform.forward, direction, Mathf.Deg2Rad * headMaxTurnAngle, 0);


            Quaternion targetRotation = Quaternion.LookRotation(direction, _transform.up) * Quaternion.Euler(90, 0, 0);
            _headBone.rotation = Quaternion.Slerp( _headBone.rotation, targetRotation, 1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime));
        }
    }
}
