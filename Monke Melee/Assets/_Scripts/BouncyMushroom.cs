using UnityEngine;
using DG.Tweening;
using System;

public class BouncyMushroom : MonoBehaviour
{
    [SerializeField] private float _strength = 1000;
    [SerializeField] private Vector3 _scaleMax = new Vector3(1, 2, 1);

    private Transform _transform;
    private Vector3 _startScale;

    private void Awake()
    {
        _transform = transform;
        _startScale = _transform.localScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check tag
        if(collision.collider.tag != "Player")
            return;

        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.AddForce(transform.up * _strength * rb.mass);

            BounceEffect();
        }
    }

    [ContextMenu("Bounce Effect")]
    private void BounceEffect()
    {
        PlaySound();
        transform.DOScale(_scaleMax, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            transform.DOScale(_startScale, 0.1f).SetEase(Ease.OutCirc);
        });
    }

    private void PlaySound()
    {
        AudioSystem.Instance.PlayGameClip((int)GameAudioEnums.Bounce, 0);
    }
}
