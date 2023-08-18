using System.Collections;
using UnityEngine;

public class Croc : MonoBehaviour
{
    [SerializeField] private float _jumpHeight = 10;
    [SerializeField] private float _firstRange = 25;

    [SerializeField] private Transform _target;

    private Rigidbody _rb;
    [SerializeField] private bool _ableToJump = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(_target == null || !_ableToJump)
            return;
        
        if(Vector3.Distance(_target.position, transform.position) < _firstRange)
        {
            _rb.AddForce((_target.position - transform.position) * _jumpHeight * _rb.mass);
            StartCoroutine(JumpTimer());
        }
    }

    [ContextMenu("AddForce")]
    public void AddForce()
    {
        _rb.AddForce((_target.position - transform.position) * _jumpHeight * _rb.mass);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _target = other.gameObject.transform;
        }
    }

    private IEnumerator JumpTimer()
    {
        _ableToJump = false;
        yield return new WaitForSeconds(5f);
        _ableToJump = true;
    }
}
