using UnityEngine;

/// <summary>
/// A bullet script used to play particle and sound effects when a bullet prefab hits an object.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitVFX;
    [SerializeField] private AudioSource _hitSFX;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject effect = Instantiate(_hitVFX.gameObject, transform.position, Quaternion.identity);
        ParticleSystem vfx = effect.GetComponent<ParticleSystem>();
        vfx.Play();
        _hitSFX.Play();
        Destroy(effect, 0.25f);
        Destroy(gameObject, 1f);
    }
}
