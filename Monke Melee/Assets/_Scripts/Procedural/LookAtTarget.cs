using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    private Transform _transform;

    [SerializeField] private Transform _target;
    [SerializeField] private Transform _headBone;

    private void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        

        if (_target != null)
        {
            Vector3 direction = (_target.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction, _transform.up);
            _headBone.rotation = Quaternion.Slerp(_headBone.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
