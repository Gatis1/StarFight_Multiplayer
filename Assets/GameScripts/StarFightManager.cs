using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StarFightManager : NetworkBehaviour
{
    [SerializeField] private Canvas NetworkUI;
    [SerializeField] private Button Host;
    [SerializeField] private Button Server;
    [SerializeField] private Button Client;

    private void Awake()
    {
        Host.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); NetworkUI.enabled = false; });
        Server.onClick.AddListener(() => { NetworkManager.Singleton.StartServer(); NetworkUI.enabled = false; });
        Client.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); NetworkUI.enabled = false; });
    }
}