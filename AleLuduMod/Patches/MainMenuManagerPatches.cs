using HarmonyLib;

namespace AleLuduMod.Patches;

internal static class MainMenuManagerPatches
{

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MainMenuManager_Start
    {
        public static void Postfix() => Utils.RemoveVanillaServer();
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Awake))]
    public static class MainMenuManager_Awake
    {
        public static void Postfix(MainMenuManager __instance) => EnterCodeManagerPatch.MainMenuManager = __instance;
    }
}
