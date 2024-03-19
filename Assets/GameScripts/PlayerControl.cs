using Unity.Netcode;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    //Attributes and additional game objects used by the player prefab.
    private float _moveSpeed = 5f;
    private int currHealth;
    private Camera _cam;
    public Rigidbody2D rig;
    public NetworkVariable<int> Health = new NetworkVariable<int>();
    [SerializeField] private ParticleSystem _deathVFX;
    [SerializeField] private AudioSource _deathSFX;
    [SerializeField] private GameObject HPBar;
    Vector2 movement;
    Vector2 aim;

    /// <summary>
    /// When the player spawns in the network, if the client does not own the prefab it is disabled so only one prefab is controlled by one player at a time.
    /// sets the player health to 3.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        rig = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
        if (!IsOwner) { this.enabled = false; }
        Health.Value = 3;
        currHealth = Health.Value;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Aim and shoot using the mouse position on the main camera.
        aim = _cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        ServerMovementRpc(movement, aim);

        if (currHealth <= 0) { Death(); }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ServerMovementRpc(Vector2 movement, Vector2 aim)
    {
        rig.velocity = movement * _moveSpeed;

        if (aim != Vector2.zero)
        {
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "PlayerShot(Clone)")
        {
            DamageRpc(1);
            ChangeHealthRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DamageRpc(int value)
    {
        currHealth -= value;
        HPBar.transform.localScale = new Vector3((float)currHealth / Health.Value, 1f, 1f);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeHealthRpc()
    {
        if (Health.Value == 0) { Death(); }
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
