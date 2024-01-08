using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; } = null;

    private ConnectionManager connectionManager;

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

    public void SetConnectionManager(ConnectionManager connectionManager)
    {
        this.connectionManager = connectionManager;

        connectionManager.OnConnectionStateChanged += OnConnectionStateChanged;
    }

    private void OnConnectionStateChanged(ConnectionState connectionState)
    {
        if (connectionState == ConnectionState.Connected)
        {
            Connected();
        }
        else if (connectionState == ConnectionState.Disconnected)
        {
            Disconnected();
        }
    }

    public void Disconnect()
    {
        connectionManager.Disconnect();
    }

    private void Connected()
    {
        SceneManager.LoadScene("GameLobby", LoadSceneMode.Single);
    }

    private void Disconnected()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
