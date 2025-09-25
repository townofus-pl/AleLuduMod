using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities;

namespace AleLuduMod;

[BepInAutoPlugin("pl.aleludumod.plugin")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency("gg.reactor.debugger", BepInDependency.DependencyFlags.SoftDependency)] // fix debugger overwriting MinPlayers
public partial class AleLuduModPlugin : BasePlugin
{
    public const int MaxPlayers = 35;
    public const int MaxImpostors = MaxPlayers / 2;
    public static ConfigEntry<bool> Force4Columns { get; set; }
    private Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        ReactorCredits.Register<AleLuduModPlugin>(ReactorCredits.AlwaysShow);

        Force4Columns = Config.Bind("Settings", "Force 4 columns", true, "Always display 4 columns in meeting, vitals, etc.");

        Harmony.PatchAll();
    }
}