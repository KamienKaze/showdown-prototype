using System;
using Unity.Netcode;
using UnityEngine;

public class UnityConnectionManager : MonoBehaviour
{
    public static UnityConnectionManager Singleton { get; private set; } = null;
    public ConnectionState connectionState;
    public Action<ConnectionState> FacepunchConnectionManager_OnConnectionStateChanged;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    public void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();

        connectionState = ConnectionState.Connected;
        FacepunchConnectionManager_OnConnectionStateChanged?.Invoke(connectionState);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;

        if (!NetworkManager.Singleton.StartClient())
        {
            return;
        }

        connectionState = ConnectionState.Connected;
        FacepunchConnectionManager_OnConnectionStateChanged?.Invoke(connectionState);
    }

    public void Disconnect()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback -=
                Singleton_OnClientConnectedCallback;
        }

        NetworkManager.Singleton.Shutdown(true);

        connectionState = ConnectionState.Disconnected;
        FacepunchConnectionManager_OnConnectionStateChanged?.Invoke(connectionState);
    }

    private void Singleton_OnServerStarted() { }

    private void Singleton_OnClientConnectedCallback(ulong clientId) { }

    private void Singleton_OnClientDisconnectCallback(ulong clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;

        if (clientId == 0)
        {
            Disconnect();
        }
    }
}
