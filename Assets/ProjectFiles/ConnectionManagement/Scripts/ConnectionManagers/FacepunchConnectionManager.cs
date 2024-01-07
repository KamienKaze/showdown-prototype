using System;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

public class FacepunchConnectionManager : MonoBehaviour
{
    public static FacepunchConnectionManager Singleton { get; private set; } = null;
    private FacepunchTransport transport = null;
    public Lobby? currentLobby { get; private set; } = null;

    public ConnectionState connectionState;
    public Action<ConnectionState> FacepunchConnectionManager_OnConnectionStateChanged;

    [SerializeField]
    private int maxMembers = 4;

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

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

        if (NetworkManager.Singleton == null)
        {
            return;
        }

        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
    }

    // Facepunch Transport Callbacks
    #region

    private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            return;
        }

        lobby.SetGameServer(lobby.Owner.Id);
        lobby.SetJoinable(true);
        lobby.SetFriendsOnly();
    }

    private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            return;
        }

        StartClient(currentLobby.Value.Owner.Id);
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend friend) { }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend friend) { }

    private void SteamMatchmaking_OnLobbyInvite(Friend friend, Lobby lobby) { }

    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        RoomEnter joinLobby = await lobby.Join();
        if (joinLobby != RoomEnter.Success)
        {
            return;
        }

        currentLobby = lobby;
    }

    #endregion

    public async void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();

        currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);

        connectionState = ConnectionState.Connected;
        FacepunchConnectionManager_OnConnectionStateChanged?.Invoke(connectionState);
    }

    public void StartClient(SteamId steamId)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;

        transport.targetSteamId = steamId;

        if (!NetworkManager.Singleton.StartClient())
        {
            return;
        }

        connectionState = ConnectionState.Connected;
        FacepunchConnectionManager_OnConnectionStateChanged?.Invoke(connectionState);
    }

    public void Disconnect()
    {
        currentLobby?.Leave();

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

    // Netcode for Gameobjects Callbacks
    #region

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

    #endregion
}
