using Newtonsoft.Json;

/// <summary>
/// The generic stuff that used to be attached to the player party, such as location, gp, etc.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class GameData {

    [JsonProperty] public Inventory Inventory { get; private set; }
    
    public GameData() {

    }
}
