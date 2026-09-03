using HarmonyLib;
using System.Linq;

namespace AleLuduMod.Patches;

internal static class GameOptionsMenuPatch
{
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    public static class GameOptionsMenu_Initialize
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            var numberOptions = __instance.GetComponentsInChildren<NumberOption>();
            var impostorsOption = numberOptions.FirstOrDefault(o => o.Title == StringNames.GameNumImpostors);
            if (impostorsOption != null)
            {
                impostorsOption.ValidRange.max = AleLuduModPlugin.MaxPlayers / 2;
            }
        }
    }
}
