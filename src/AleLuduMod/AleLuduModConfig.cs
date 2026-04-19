using BepInEx.Configuration;

namespace AleLuduMod;

public static class AleLuduModConfig
{
    public static ConfigEntry<bool> Force4Columns { get; set; }

    public static void Bind(ConfigFile config)
    {
        Force4Columns = config.Bind("Settings", "Force 4 Columns", false, "Always display 4 columns in meeting, vitals & shapeshifter menu");
    }
}