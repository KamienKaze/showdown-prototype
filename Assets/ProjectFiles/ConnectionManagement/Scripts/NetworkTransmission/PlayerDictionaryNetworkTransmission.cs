using Unity.Netcode;

public class PlayerDictionaryNetworkTransmission : NetworkBehaviour
{
    public static PlayerDictionaryNetworkTransmission Singleton { get; private set; } = null;

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

    [ServerRpc(RequireOwnership = false)]
    public void AddMeToDictionaryServerRPC(ulong clientId, ulong steamId, string steamName)
    {
        AddPlayerToDictionaryClientRPC(clientId, steamId, steamName);
    }

    [ClientRpc]
    private void AddPlayerToDictionaryClientRPC(ulong clientId, ulong steamId, string steamName)
    {
        PlayerDictionaryManager.Singleton.AddPlayerToDictionary(clientId, steamId, steamName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveMeFromDictionaryServerRPC(ulong clientId)
    {
        RemovePlayerFromDictionaryClientRPC(clientId);
    }

    [ClientRpc]
    private void RemovePlayerFromDictionaryClientRPC(ulong clientId)
    {
        PlayerDictionaryManager.Singleton.RemovePlayerFromDictionary(clientId);
    }
}
