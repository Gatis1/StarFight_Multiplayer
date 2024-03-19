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
        rig = GetComponent<Rigidbody2D>();
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

        if (IsOwner) { ServerMovementRpc(movement, aim); }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ServerMovementRpc(Vector2 movement, Vector2 aim)
    {
        //moves the character based on the postion of the "move" stick.
        rig.velocity = movement * _moveSpeed;

        //rotates the character based on he position of the "aim" stick.
        if (aim != Vector2.zero)
        {
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0,0,angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "PlayerShot(Clone)") 
        { 
            DamageRpc(1);
            ChangeHealthRpc();
        }
        else 
        { 
            DamageRpc(3);
            ChangeHealthRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DamageRpc(int value)
    {
        Health.Value -= value;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeHealthRpc()
    {
        if(Health.Value == 0) { Death(); }
    }

    private void Death()
    {
        GameObject effect = Instantiate(_deathVFX.gameObject, transform.position, Quaternion.identity);
        ParticleSystem vfx = effect.GetComponent<ParticleSystem>();
        vfx.Play();
        _deathSFX.Play();
        Destroy(effect, 2f);
        NetworkObject.Despawn(gameObject);
    }
}
