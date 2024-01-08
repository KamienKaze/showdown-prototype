using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; } = null;

    public NetworkTransport currentNetworkTransport;

    [HideInInspector]
    public ConnectionManager currentConnectionManager;

    [Header("Network Transport Manager Objects")]
    public GameObject facepunchTransportManager;
    public GameObject unityTransportManager;

    private InputManager playerInputManager;
    private SceneLoader sceneLoader;

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
        playerInputManager = GetComponent<InputManager>();
        sceneLoader = GetComponent<SceneLoader>();

        SetConnectionManager(currentNetworkTransport);
    }

    // Connection Manager
    #region

    public void SetConnectionManager(NetworkTransport networkTransport)
    {
        switch (networkTransport)
        {
            case NetworkTransport.Facepunch:
                currentConnectionManager =
                    facepunchTransportManager.GetComponent<FacepunchConnectionManager>();

                facepunchTransportManager.SetActive(true);
                Destroy(unityTransportManager);
                break;

            case NetworkTransport.Unity:
                currentConnectionManager =
                    unityTransportManager.GetComponent<UnityConnectionManager>();

                unityTransportManager.SetActive(true);
                Destroy(facepunchTransportManager);
                break;
        }

        currentConnectionManager.OnConnectionStateChanged += HandleConnectionStateChange;
    }

    private void HandleConnectionStateChange(ConnectionState connectionState)
    {
        switch (connectionState)
        {
            case ConnectionState.Connected:
                sceneLoader.LoadMenuScene(MenuScene.GameLobby);
                break;

            case ConnectionState.Disconnected:
                sceneLoader.LoadMenuScene(MenuScene.MainMenu);
                break;
        }
    }

    #endregion

    // Input Manager
    #region

    public InputManager GetInputManager()
    {
        return playerInputManager;
    }

    #endregion
}
