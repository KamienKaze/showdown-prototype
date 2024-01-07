public class PlayerInformation
{
    private ulong steamId;
    private string steamName;

    public PlayerInformation(ulong steamId, string steamName)
    {
        this.steamId = steamId;
        this.steamName = steamName;
    }

    public ulong SteamId { get; }
    public string SteamName { get; }
}
