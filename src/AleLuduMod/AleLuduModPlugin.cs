using AmongUs.GameOptions;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using BepInEx.Configuration;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using System.Linq;
using BepInEx.Logging;

namespace AleLuduMod;

[BepInAutoPlugin("pl.townofus.aleludu")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency("gg.reactor.debugger", BepInDependency.DependencyFlags.SoftDependency)] // fix debugger overwriting MinPlayers
[BepInDependency("com.slushiegoose.townofus", BepInDependency.DependencyFlags.SoftDependency)] // load after town of us
public partial class AleLuduModPlugin : BasePlugin
{
    public const int MaxPlayers = 35;
    public const int MaxImpostors = 35 / 2;
    public static ConfigEntry<bool> Force4Columns { get; set; }
    private Harmony Harmony { get; } = new(Id);

    public override void Load()
    { 
        NormalGameOptionsV09.RecommendedImpostors = NormalGameOptionsV09.MaxImpostors = Enumerable.Repeat(35, 35).ToArray();
        NormalGameOptionsV09.MinPlayers = Enumerable.Repeat(4, 35).ToArray();
        HideNSeekGameOptionsV09.MinPlayers = Enumerable.Repeat(4, 35).ToArray();

        Force4Columns = Config.Bind("Settings", "Force 4 columns", true, "Always display 4 columns in meeting, vitals, etc.");

        Harmony.PatchAll();
    }
}