using System.Collections;
using UnityEngine;

public class FootStepper : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private Vector3 _defaultPosition;

    public Vector3 FootPosition;

    public bool Moving;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        // Get _wantMoveDistance, _distanceToGround, and other constant variables from whatever script is controlling this foot
        FootPosition = _transform.position;
    }

    private void Update()
    {
        _transform.position = FootPosition;
    }

    public void TryStep(RaycastHit hit, float _wantMoveDistance, float _distanceToGround, float _moveDuration, float _stepOvershootFraction)
    {
        if(Moving) return;

        float dist = Vector3.Distance(FootPosition, hit.point);
        if (dist > _wantMoveDistance)
            StartCoroutine(Step(hit, _wantMoveDistance, _distanceToGround, _moveDuration, _stepOvershootFraction));
    }

    public IEnumerator Step(RaycastHit hit, float _wantMoveDistance, float _distanceToGround, float _moveDuration, float _stepOvershootFraction)
    {
        if (hit.collider == null)
            yield break;

        Moving = true;

        Vector3 startPoint = _transform.position;
        Quaternion startRot = _transform.rotation;

        Vector3 newFootPosition = hit.point;
        newFootPosition.y += _distanceToGround;

        // Overshoot
        Vector3 towardTarget = (newFootPosition - FootPosition);
        float overshootDistance = _wantMoveDistance * _stepOvershootFraction;
        Vector3 overshootVector = towardTarget * overshootDistance;
        // overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPoint = newFootPosition + overshootVector;
        Quaternion endRot = Quaternion.LookRotation(hit.normal, _transform.forward);

        Vector3 centerPoint = (startPoint + endPoint) / 2;
        float verticalDistance = Vector3.Distance(startPoint, endPoint) / 2;
        verticalDistance = Mathf.Clamp(verticalDistance, 0, 0.25f);
        centerPoint += _transform.up * verticalDistance;

        // Interpolate over curve
        float timeElapsed = 0;
        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / _moveDuration;

            FootPosition =
            Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime
            );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < _moveDuration);

        Moving = false;
    }
}