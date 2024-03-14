using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    PlayerControl playerScript;
    public Transform firePoint;
    public GameObject PlayerShot;
    [SerializeField] private AudioSource pew;
    public float shotForce = 20f;
    public float fireRate = 1f;
    public float FireTime = 0f;

    void Start()
    {
        playerScript = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1") && Time.time > FireTime)
        {
            FireTime = Time.time + 1f / fireRate;
            pew.Play();
            Fire();
        }
    }

    void Fire()
    {
        GameObject shot = Instantiate(PlayerShot, firePoint.position, firePoint.rotation);
        Rigidbody2D rig = shot.GetComponent<Rigidbody2D>();
        rig.AddForce(-firePoint.up * shotForce, ForceMode2D.Impulse);
    }
}
