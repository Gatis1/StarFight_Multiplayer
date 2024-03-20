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
    [SerializeField] private AudioSource _criticalSFX;
    [SerializeField] private AudioSource _moveSFX;
    [SerializeField] private AudioSource _ramSFX;
    [SerializeField] private GameObject HPBar;
    public bool alive;
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
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        //Aim and shoot using the mouse position on the main camera.
        aim = _cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        //Passes movement and aim to a function that syncs client movement to the server
        ServerMovementRpc(movement, aim);
    }

    //Rpc used to send and sync data to clients and host
    [Rpc(SendTo.ClientsAndHost)]
    private void ServerMovementRpc(Vector2 movement, Vector2 aim)
    {
        //moves the player prefab based on movement inputs and movement speed
        rig.velocity = movement * _moveSpeed;
        _moveSFX.Play();

        //rotates the player prefab to be ontrack with the mouse position
        if (aim != Vector2.zero)
        {
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    //trigger check to see if the player projectile hits another player.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "PlayerShot(Clone)")
        {
            //calls damage function and sync function
            DamageRpc(1);
            ChangeHealthRpc();
        }
        if(collision.gameObject.name == "PlayerShip(Clone)")
        {
            _ramSFX.Play();
            DamageRpc(2);
            ChangeHealthRpc();
        }
    }

    //syncs the new values of a player's health to the server and changes the UI element that shows the current player health
    [Rpc(SendTo.ClientsAndHost)]
    private void DamageRpc(int value)
    {
        currHealth -= value;
        HPBar.transform.localScale = new Vector3((float)currHealth / Health.Value, 1f, 1f);
    }

    //Sync function to check the current health of a player.
    //Calls the death function if current health is less than or equal to 0.
    //If the current health of the player is 1 a critical tone is played.
    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeHealthRpc()
    {
        if(currHealth == 1) { _criticalSFX.Play(); }

        if (currHealth <= 0) { Death(); alive = false; }
    }

    //Plays a particle effect and sound effect upon player "death"
    private void Death()
    {
        GameObject effect = Instantiate(_deathVFX.gameObject, transform.position, Quaternion.identity);
        ParticleSystem vfx = effect.GetComponent<ParticleSystem>();
        vfx.Play();
        _deathSFX.Play();
        Destroy(effect, 1f);
        gameObject.SetActive(false);
    }
}
