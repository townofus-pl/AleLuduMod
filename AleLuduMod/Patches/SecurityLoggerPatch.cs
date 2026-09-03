using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AleLuduMod.Patches;

internal static class SecurityLoggerPatch
{
    [HarmonyPatch(typeof(SecurityLogger), nameof(SecurityLogger.Awake))]
    public static class SecurityLoggerAwakePatch
    {
        public static void Postfix(SecurityLogger __instance)
        {
            __instance.Timers = new Il2CppStructArray<float>(AleLuduModPlugin.MaxPlayers);
            Info("Patched SecurityLogger Timers");
        }
    }
}