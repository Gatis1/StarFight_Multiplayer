using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

//A short script to wirte the player name as Player1 (P1), Player2 (P2), etc. as the players spawn in the network.
public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    private NetworkVariable<FixedString128Bytes> _netWorkPlayerName = new NetworkVariable<FixedString128Bytes>("P0");

    public override void OnNetworkSpawn()
    {
        //Add 1 to the client ID since it starts counting at 0.
        _netWorkPlayerName.Value = "P" + (OwnerClientId + 1);
        //Append the player name from the network to the player UI text.
        _playerName.text = _netWorkPlayerName.Value.ToString();
    }


}
