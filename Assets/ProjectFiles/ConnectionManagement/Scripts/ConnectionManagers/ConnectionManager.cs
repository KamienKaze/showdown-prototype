using System;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

public enum ConnectionState
{
    Connected,
    Disconnected,
}

public class ConnectionManager : MonoBehaviour
{
    public Action<ConnectionState> OnConnectionStateChanged;

    protected Action HostStarted;
    protected Action ClientStarted;
    protected Action Disconnected;

    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();

        OnConnectionStateChanged?.Invoke(ConnectionState.Connected);

        HostStarted?.Invoke();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;

        ClientStarted?.Invoke();

        if (!NetworkManager.Singleton.StartClient())
        {
            return;
        }

        OnConnectionStateChanged?.Invoke(ConnectionState.Connected);
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

        OnConnectionStateChanged?.Invoke(ConnectionState.Disconnected);
        Disconnected?.Invoke();

        NetworkManager.Singleton.Shutdown(true);
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
