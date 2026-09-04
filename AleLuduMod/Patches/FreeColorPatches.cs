using HarmonyLib;

namespace AleLuduMod.Patches;

// By default, the FreeColor option is disabled; you must change the configuration in the "BepInEx\config\pl.townofus.aleludu.cfg" file.
// This option will not be available in the standard settings, as it is an experimental feature.
internal static class FreeColorPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
    public static class PlayerControlCheckColorPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte colorId)
        {
            if (!AleLuduModConfig.FreeColor.Value) return true; 

            __instance.RpcSetColor(colorId);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.Update))]
    public static class PlayerTabIsSelectedItemEquippedPatch
    {
        public static void Postfix(PlayerTab __instance)
        {
            if (!AleLuduModConfig.FreeColor.Value) return;

            __instance.currentColorIsEquipped = false;
        }
    }

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
    public static class PlayerTabUpdateAvailableColorsPatch
    {
        public static bool Prefix(PlayerTab __instance)
        {
            if (!AleLuduModConfig.FreeColor.Value) return true;

            __instance.AvailableColors.Clear();

            for (var i = 0; i < Palette.PlayerColors.Count; i++)
            {
                if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.CurrentOutfit.ColorId != i)
                    __instance.AvailableColors.Add(i);
            }

            return false;
        }
    }
}