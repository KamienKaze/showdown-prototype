using System;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public ConnectionState connectionState;
    public Action<ConnectionState> ConnectionManager_OnConnectionStateChanged;
}
