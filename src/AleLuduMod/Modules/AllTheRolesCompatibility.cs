using BepInEx.Unity.IL2CPP;
using System.Reflection;

namespace AleLuduMod.Modules;

public static class AllTheRolesCompatibility
{
    public const string AllTheRolesGuid = "com.zeo.alltheroles";
    public static bool AllTheRolesIsLoaded { get; private set; }
    private static BasePlugin Plugin { get; set; }
    private static Assembly AllTheRolesAssembly { get; set; }

    public static void Initialize() => InitAllTheRoles();

    private static void InitAllTheRoles()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(AllTheRolesGuid, out var value)) return;

        Plugin = (value.Instance as BasePlugin)!;
        AllTheRolesAssembly = Plugin.GetType().Assembly;

        AllTheRolesIsLoaded = true;
        AleLuduLogger.Info("All The Roles has successfully loaded!");
    }
}