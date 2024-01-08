using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

public class FacepunchConnectionManager : ConnectionManager
{
    public static FacepunchConnectionManager Singleton { get; private set; } = null;
    private FacepunchTransport transport = null;
    public Lobby? currentLobby { get; private set; } = null;

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
            Destroy(gameObject);
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

        HostStarted += OnHostStarted;
        ClientStarted += OnClientStarted;
        Disconnected += OnDisconnected;
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

        HostStarted -= OnHostStarted;
        ClientStarted -= OnClientStarted;
        Disconnected -= OnDisconnected;
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

        StartClient();
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

    public async void OnHostStarted()
    {
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
    }

    public void OnClientStarted()
    {
        transport.targetSteamId = currentLobby.Value.Owner.Id;
    }

    public void OnDisconnected()
    {
        currentLobby = null;
    }
}
