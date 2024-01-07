using System;
using UnityEngine;

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
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        connectionManager.ConnectionManager_OnConnectionStateChanged += HandleConnectionStateChange;
    }

    private void HandleConnectionStateChange(ConnectionState connectionState) { }

    public void SetConnectionManager(ConnectionManager connectionManager)
    {
        this.connectionManager = connectionManager;
    }
}
