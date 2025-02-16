using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;

namespace AleLuduMod.Patches;

internal static class GenericPatches
{


    // I did not find a use of this method, but still patching for future updates
    // maxExpectedPlayers is unknown, looks like server code tbh
    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.AreInvalid))]
    public static class InvalidOptionsPatches
    {
        public static bool Prefix(GameOptionsData __instance, [HarmonyArgument(0)] int maxExpectedPlayers)
        {
            return __instance.MaxPlayers > maxExpectedPlayers ||
                   __instance.NumImpostors < 1 ||
                   __instance.NumImpostors + 1 > maxExpectedPlayers / 2 ||
                   __instance.KillDistance is < 0 or > 2 ||
                   __instance.PlayerSpeedMod is <= 0f or > 3f;
        }
    }


    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingShowerPatch
    {
        public static void Postfix(PingTracker __instance)
        {
            __instance.text.text += "\n<color=#FFB793>townofus.pl</color>";
        }
    }


    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    public static class GameOptionsMenu_Initialize
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            var numberOptions = __instance.GetComponentsInChildren<NumberOption>();

            var impostorsOption = numberOptions.FirstOrDefault(o => o.Title == StringNames.GameNumImpostors);
            if (impostorsOption != null)
            {
                impostorsOption.ValidRange = new FloatRange(1, AleLuduModPlugin.MaxImpostors);
            }

        }
    }
}
