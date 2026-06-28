using BepInEx.Unity.IL2CPP;
using System.Reflection;

namespace AleLuduMod.Modules;

public static class TheOtherRolesCompatibility
{
    public const string TheOtherRolesGuid = "me.eisbison.theotherroles";
    public static bool TheOtherRolesIsLoaded { get; private set; }
    private static BasePlugin TheOtherRolesPlugin { get; set; }
    private static Assembly TheOtherRolesAssembly { get; set; }

    public static void Initialize() => InitTheOtherRoles();

    private static void InitTheOtherRoles()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(TheOtherRolesGuid, out var value)) return;

        TheOtherRolesPlugin = (value.Instance as BasePlugin)!;
        TheOtherRolesAssembly = TheOtherRolesPlugin.GetType().Assembly;

        TheOtherRolesIsLoaded = true;
        AleLuduLogger.Info("The Other Roles has successfully loaded!");
    }
}