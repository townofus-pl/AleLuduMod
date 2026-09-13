using HarmonyLib;
using System.Linq;

namespace AleLuduMod;

internal static class RemoveInnerslothRegions
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class RemoveVanillaServerPatch
    {
        public static void Postfix() => RemoveVanillaServer();
    }

    public static void RemoveVanillaServer()
    {
        var sm = ServerManager.Instance;
        var curRegions = sm.AvailableRegions;
        sm.AvailableRegions = curRegions.Where(region => !IsVanillaServer(region)).ToArray();

        var defaultRegion = ServerManager.DefaultRegions;
        ServerManager.DefaultRegions = defaultRegion.Where(region => !IsVanillaServer(region)).ToArray();

        if (IsVanillaServer(sm.CurrentRegion))
        {
            var region = defaultRegion.FirstOrDefault();
            sm.SetRegion(region);
        }

        Info("Finished removing Vanilla Servers!");
    }

    private static bool IsVanillaServer(IRegionInfo? regionInfo)
    {
        return regionInfo is
        {
            TranslateName:
            StringNames.ServerAS or
            StringNames.ServerEU or
            StringNames.ServerNA
        };
    }
}