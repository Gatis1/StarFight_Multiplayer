using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A short script to anchor the PlayerName to the top of the player prefab and keep it from rotating.
public class UIManager : MonoBehaviour
{
    public Transform playerPos;
    public Vector3 offset;
    Quaternion rotation;

    //Gets the initial rotation position then the player UI instantiates with the player.
    private void Awake()
    {
        rotation = transform.rotation;
    }

    /// <summary>
    /// sets the rotation of the player name text to the initial rotation position as the player moves.
    /// adds an offset value to keep the player name above the player prefab.
    /// </summary>
    private void LateUpdate()
    {
        transform.rotation = rotation;

        transform.position = playerPos.position + offset;
    }
}
