using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;

namespace AleLuduMod;

[BepInAutoPlugin("pl.townofus.aleludu", "AleLuduMod")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnHost)]
[BepInIncompatibility("xyz.crowdedmods.crowdedmod")] // CrowdedMod is incompatible, because it modifies the interface of the Meeting / Vitals / Shapeshifter Menu.
[BepInIncompatibility("dev.allofus.overloaded")] // Overloaded is incompatible, because it modifies the interface of the Meeting / Vitals / Shapeshifter Menu.
public partial class AleLuduModPlugin : BasePlugin
{
    public const int MaxPlayers = 127; // In Classic, it is recommended to set a number <= 28. Does not apply to HideNSeek.

    private Harmony Harmony { get; } = new(Id);

    public static bool IsDevBuild => true;

    public override void Load()
    {
        AleLuduModConfig.Bind(Config);

        ReactorCredits.Register("AleLuduMod", Version, IsDevBuild, ReactorCredits.AlwaysShow);

        ModCompatibility.Loaded();

        Harmony.PatchAll();
    }
}