using System.Collections;
using UnityEngine;

public class FootStepper : MonoBehaviour
{
    private Transform _transform;

    public Vector3 FootPosition;

    public bool Moving;

    [SerializeField] private float _moveDuration = 0.1f;
    [SerializeField] private float _stepOvershootFraction = 1;


    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        // Get _wantMoveDistance, _distanceToGround, and other constant variables from whatever script is controlling this foot
    }

    public void TryStep(RaycastHit hit, float _wantMoveDistance, float _distanceToGround)
    {
        if (Moving) return;

        float distFromHome = Vector3.Distance(FootPosition, hit.point);

        // If we are too far off in position or rotation
        if (distFromHome > _wantMoveDistance)
        {
            StartCoroutine(Step(hit, _wantMoveDistance, _distanceToGround));
        }
    }

    public IEnumerator Step(RaycastHit hit, float _wantMoveDistance, float _distanceToGround)
    {
        Moving = true;

        Vector3 startPoint = _transform.position;
        // Quaternion startRot = transform.rotation;

        Vector3 newFootPosition = hit.point;
        newFootPosition.y += _distanceToGround;

        Vector3 towardHome = (newFootPosition - FootPosition);

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
            FootPosition =
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
