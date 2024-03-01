using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rig;
    //public Camera camera;
    Vector2 movement;
    Vector2 aim;
    bool fire;

    [Header("Joystick")]
    public Joystick MVjoystick;
    public Joystick AIMjoystick;

    // Update is called once per frame
    void Update()
    {
        //movement
        //Gets the x and y position of the "move" stick to register 360 movement.
        movement.x = MVjoystick.Horizontal;
        movement.y = MVjoystick.Vertical;

        //Aim and shoot.
        //When the "aim" stick moves away form the deadzone of (0,0) rotation registers.
        if (AIMjoystick.Horizontal != 0 || AIMjoystick.Vertical != 0)
        {
            aim = new Vector2(AIMjoystick.Horizontal, AIMjoystick.Vertical);
        }
        //When the "aim" stick reaches a certian threshold in the circle change fire variable to true to shoot a bullet.
        float magnitude = aim.magnitude;
        if(magnitude >= 0.75)
        {
            fire =  true;
        }
        else
        {
            fire = false;
        }
        //aim = camera.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        //moves the character based on the postion of the "move" stick.
        rig.MovePosition(rig.position + movement * moveSpeed * Time.fixedDeltaTime);

        //rotates the character based on he position of the "aim" stick.
        if (aim != Vector2.zero)
        {
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + 90f;
            rig.rotation = angle;
        }
    }

    public bool getFire()
    {
        return fire;
    }
}
