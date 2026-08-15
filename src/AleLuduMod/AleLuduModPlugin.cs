using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities;

namespace AleLuduMod;

[BepInAutoPlugin("pl.townofus.aleludu")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInIncompatibility("xyz.crowdedmods.crowdedmod")] // CrowdedMod is incompatible, because it modifies the interface of the Meeting / Vitals / Shapeshifter Menu.
public partial class AleLuduModPlugin : BasePlugin
{
    public const int MaxPlayers = 35;
    public const int MaxImpostors = MaxPlayers / 2;
    private Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        AleLuduModConfig.Bind(Config);

        ReactorCredits.Register<AleLuduModPlugin>(ReactorCredits.AlwaysShow);

        ModCompatibility.Loaded();

        Harmony.PatchAll();
    }
}