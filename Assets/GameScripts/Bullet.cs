using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitVFX;
    [SerializeField] private AudioSource _hitSFX;

    private void Update()
    {
        Destroy(gameObject, 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject effect = Instantiate(_hitVFX.gameObject, transform.position, Quaternion.identity);
        ParticleSystem vfx = effect.GetComponent<ParticleSystem>();
        vfx.Play();
        _hitSFX.Play();
        Destroy(effect, 1f);
        Destroy(gameObject);
    }
}
