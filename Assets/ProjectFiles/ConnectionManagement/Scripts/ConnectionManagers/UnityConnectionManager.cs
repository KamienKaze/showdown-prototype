using Unity.Netcode;

public class UnityConnectionManager : ConnectionManager
{
    public static UnityConnectionManager Singleton { get; private set; } = null;

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

        OnConnectionStateChanged?.Invoke(ConnectionState.Connected);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;

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
