using System.Collections;
using UnityEngine;

public class CrocMouthController : MonoBehaviour
{
    // Properties
    public Transform MouthBone => _mouthBone;

    // Serialized Fields
    [Range(0,1)] private float _t = 0;

    [Range(0,1)][SerializeField] private float _debugT;
    [SerializeField] private float _debugDuration = 1;

    [SerializeField] private Transform _mouthBone;
    [SerializeField] private Transform _jawBone;
    [SerializeField] private Vector3 _jawOpenRotation;
    [SerializeField] private Vector3 _jawClosedRotation;
    [SerializeField] private Vector3 _mouthOpenRotation;
    [SerializeField] private Vector3 _mouthClosedRotation;

    [ContextMenu("Set Mouth")]
    public void SetMouth()
    {
        StartCoroutine(OpenCloseMouthCoroutine(_debugT, _debugDuration));
    }

    private void OpenCloseMouth()
    {
        _mouthBone.localRotation = Quaternion.Lerp(Quaternion.Euler(_mouthClosedRotation), Quaternion.Euler(_mouthOpenRotation), _t);
        _jawBone.localRotation = Quaternion.Lerp(Quaternion.Euler(_jawClosedRotation), Quaternion.Euler(_jawOpenRotation), _t);
    }

    private IEnumerator OpenCloseMouthCoroutine(float targetOpenness, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            _t = Mathf.Lerp(_t, targetOpenness, elapsedTime / duration);
            OpenCloseMouth();
            yield return null;
        }

        // Ensure the final openness value is accurately set
        _t = targetOpenness;
        OpenCloseMouth();
    }

    // float t;
    // private IEnumerator OpenCloseMouth(float targetT)
    // {
    //     while (t < targetT)
    //     {
    //         t += Time.deltaTime * 5f;
    //         _mouthBone.localRotation = Quaternion.Lerp(Quaternion.Euler(_mouthClosedRotation), Quaternion.Euler(_mouthOpenRotation), t);
    //         _jawBone.localRotation = Quaternion.Lerp(Quaternion.Euler(_jawClosedRotation), Quaternion.Euler(_jawOpenRotation), t);
    //         yield return null;
    //     }

    // }

    // public IEnumerator OpenCloseMouth(float targetT)
    // {
    //     float elapsedTime = 0f;

    //     while (elapsedTime < targetT)
    //     {
    //         yourTransform.rotation = Quaternion.Euler(Vector3.Lerp(openEulerAngle, closedEulerAngle, elapsedTime / targetT));
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     yourTransform.rotation = Quaternion.Euler(closedEulerAngle);
    // }
}
