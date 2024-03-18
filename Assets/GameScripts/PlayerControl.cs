using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerControl : NetworkBehaviour
{
    private float _moveSpeed = 5f;
    private Camera _cam;
    public Rigidbody2D rig;
    [SerializeField] private int _health = 3;
    Vector2 movement;
    Vector2 aim;

    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();


    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            Move();
        }
    }

    public void Move()
    {
        SubmitPositionRequestServerRpc();
    }

    [Rpc(SendTo.Server)]
    void SubmitPositionRequestServerRpc(RpcParams rpcParams = default)
    {
        transform.position = Position.Value;
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
        aim = _cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        //moves the character based on the postion of the "move" stick.
        rig.velocity = movement * _moveSpeed;

        //rotates the character based on he position of the "aim" stick.
        if (aim != Vector2.zero)
        {
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + 90f;
            rig.rotation = angle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _health -= 1;
    }
}
