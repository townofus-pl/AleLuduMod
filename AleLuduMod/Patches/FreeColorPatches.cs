using HarmonyLib;

namespace AleLuduMod.Patches;

internal static class FreeColorPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
    public static class PlayerControlCheckColorPatch
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte colorId)
        {
            __instance.RpcSetColor(colorId);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.Update))]
    public static class PlayerTabIsSelectedItemEquippedPatch
    {
        public static void Postfix(PlayerTab __instance)
        {
            __instance.currentColorIsEquipped = false;
        }
    }

    [HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
    public static class PlayerTabUpdateAvailableColorsPatch
    {
        public static bool Prefix(PlayerTab __instance)
        {
            __instance.AvailableColors.Clear();
            for (var i = 0; i < Palette.PlayerColors.Count; i++)
            {
                if (!PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.CurrentOutfit.ColorId != i)
                {
                    __instance.AvailableColors.Add(i);
                }
            }

            return false;
        }
    }
}
