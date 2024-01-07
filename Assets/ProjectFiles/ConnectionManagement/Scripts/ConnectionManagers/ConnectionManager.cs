using System;
using UnityEngine;

public enum ConnectionState
{
    Connected,
    Disconnected,
}

public class ConnectionManager : MonoBehaviour
{
    public Action<ConnectionState> OnConnectionStateChanged;
}
