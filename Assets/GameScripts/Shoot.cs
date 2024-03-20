using UnityEngine;
using Unity.Netcode;

/// <summary>
/// A function that fires a projectile from the player prefab.
/// </summary>
public class Shoot : NetworkBehaviour 
{
    public Transform _firePoint;
    public GameObject _playerShot;
    [SerializeField] private AudioSource _pew;
    public float shotForce = 20f;
    public float fireRate = 1f;
    public float FireTime = 0f;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { Destroy(this); }
    }

    // Update is called once per frame
    void Update()
    {
        //The time and FireTime variables are used to limit the amount of shots fired from the player to one projectile at a time.
        if (Input.GetButtonDown("Fire1") && Time.time > FireTime)
        {
            FireTime = Time.time + 1f / fireRate;
            _pew.Play();
            Fire();
        }
    }

    void Fire()
    {
        GameObject shot = Instantiate(_playerShot, _firePoint.position, _firePoint.rotation);
        Rigidbody2D rig = shot.GetComponent<Rigidbody2D>();
        rig.AddForce(-_firePoint.up * shotForce, ForceMode2D.Impulse);
    }
}
