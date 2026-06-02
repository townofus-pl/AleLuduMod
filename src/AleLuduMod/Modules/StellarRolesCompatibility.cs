using BepInEx.Unity.IL2CPP;
using System.Reflection;

namespace AleLuduMod.Modules;

public static class StellarRolesCompatibility
{
    public const string StellarRolesGuid = "me.fluff.stellarroles";
    public static bool StellarRolesIsLoaded { get; private set; }
    private static BasePlugin StellarRolesPlugin { get; set; }
    private static Assembly StellarRolesAssembly { get; set; }

    public static void Initialize() => InitStellarRoles();

    private static void InitStellarRoles()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(StellarRolesGuid, out var value)) return;

        StellarRolesPlugin = (value.Instance as BasePlugin)!;
        StellarRolesAssembly = StellarRolesPlugin.GetType().Assembly;

        StellarRolesIsLoaded = true;
        AleLuduLogger.Info("Stellar Roles has successfully loaded!");
    }
}
