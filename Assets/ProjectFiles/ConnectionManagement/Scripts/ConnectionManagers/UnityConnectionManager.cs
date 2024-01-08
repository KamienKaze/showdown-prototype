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
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        HostStarted += OnHostStarted;
        ClientStarted += OnClientStarted;
        Disconnected += OnDisconnected;
    }

    private void OnDestroy()
    {
        HostStarted -= OnHostStarted;
        ClientStarted -= OnClientStarted;
        Disconnected -= OnDisconnected;
    }

    public void OnHostStarted() { }

    public void OnClientStarted() { }

    public void OnDisconnected() { }
}
