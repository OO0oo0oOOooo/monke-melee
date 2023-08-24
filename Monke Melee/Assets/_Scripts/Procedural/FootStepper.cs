using System.Collections;
using UnityEngine;

public class FootStepper : MonoBehaviour
{
    private Transform _transform;

    public bool Moving;

    [SerializeField] private float _moveDuration = 0.1f;
    [SerializeField] private float _stepOvershootFraction = 1;

    private void Awake()
    {
        _transform = transform;
    }

    public IEnumerator Step(RaycastHit hit, float _wantMoveDistance, float _distanceToGround)
    {
        Moving = true;

        Vector3 startPoint = _transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 newFootPosition = hit.point;
        newFootPosition.y += _distanceToGround;

        Vector3 towardHome = (newFootPosition - _transform.position);

        float overshootDistance = _wantMoveDistance * _stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPoint = hit.point + overshootVector;

        // Ground normal rotation
        // Quaternion endRot = homeTransform.rotation;

        Vector3 centerPoint = (startPoint + endPoint) / 2;
        centerPoint += _transform.up * Vector3.Distance(startPoint, endPoint) / 2f;

        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / _moveDuration;

            // Quadratic bezier curve
            _transform.position =
            Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime
            );

            // transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < _moveDuration);

        Moving = false;
    }
}