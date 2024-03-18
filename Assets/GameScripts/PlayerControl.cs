using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    private float _moveSpeed = 5f;
    private Camera _cam;
    public Rigidbody2D rig;
    [SerializeField] private NetworkVariable<int> Health = new NetworkVariable<int>(3);
    [SerializeField] private ParticleSystem _deathVFX;
    [SerializeField] private AudioSource _deathSFX;
    Vector2 movement;
    Vector2 aim;

    public override void OnNetworkSpawn()
    {
        _cam = Camera.main;
        if (!IsOwner) { this.enabled = false; }
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Aim and shoot.
        aim = _cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (Health.Value == 0) { Death(); }

        if (IsOwner) { UpdateMovementServerRpc(movement, aim); }
    }

    [ServerRpc]
    private void UpdateMovementServerRpc(Vector2 movement, Vector2 aim)
    {
        //moves the character based on the postion of the "move" stick.
        rig.velocity = movement * _moveSpeed;

        //rotates the character based on he position of the "aim" stick.
        if (aim != Vector2.zero)
        {
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + 90f;
            rig.rotation = angle;
        }

        UpdateMovementClientRpc(rig.position, rig.rotation);
    }

    [ClientRpc]
    private void UpdateMovementClientRpc(Vector2 position, float rotation)
    {
        rig.position = position;
        rig.rotation = rotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "PlayerShot") 
        {
            if (IsOwner) { Health.Value -= 1; } else { DamageServerRpc(); }
        }
        else { Health.Value -= 3; }
    }

    [ServerRpc]
    private void DamageServerRpc()
    {
        Health.Value -= 1;
    }

    private void Death()
    {
        GameObject effect = Instantiate(_deathVFX.gameObject, transform.position, Quaternion.identity);
        ParticleSystem vfx = effect.GetComponent<ParticleSystem>();
        vfx.Play();
        _deathSFX.Play();
        Destroy(effect, 1f);
        NetworkObject.Despawn(gameObject);
    }
}
