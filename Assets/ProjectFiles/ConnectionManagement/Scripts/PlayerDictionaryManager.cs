using System.Collections.Generic;
using UnityEngine;

public class PlayerDictionaryManager : MonoBehaviour
{
    public static PlayerDictionaryManager Singleton { get; private set; } = null;

    private Dictionary<ulong, PlayerInformation> playerDictionary =
        new Dictionary<ulong, PlayerInformation>();

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

    public Dictionary<ulong, PlayerInformation> GetAllPlayers()
    {
        return playerDictionary;
    }

    public KeyValuePair<ulong, PlayerInformation>? GetPlayerBySteamId(ulong steamId)
    {
        foreach (KeyValuePair<ulong, PlayerInformation> player in playerDictionary)
        {
            if (player.Value.SteamId == steamId)
            {
                return player;
            }
        }

        return null;
    }

    public KeyValuePair<ulong, PlayerInformation>? GetPlayerByClientId(ulong clientId)
    {
        foreach (KeyValuePair<ulong, PlayerInformation> player in playerDictionary)
        {
            if (player.Key == clientId)
            {
                return player;
            }
        }

        return null;
    }

    public void AddPlayerToDictionary(ulong clientId, ulong steamId, string steamName)
    {
        playerDictionary.Add(clientId, new PlayerInformation(steamId, steamName));
    }

    public void RemovePlayerFromDictionary(ulong clientId)
    {
        if (GetPlayerByClientId(clientId) != null)
        {
            playerDictionary.Remove(GetPlayerByClientId(clientId).Value.Key);
        }
    }

    private void ClearDictionary()
    {
        playerDictionary.Clear();
    }
}
