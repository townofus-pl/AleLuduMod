using AleLuduMod.Components;
using HarmonyLib;

namespace AleLuduMod.Patches;

internal static class PagingPatches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingHudStartPatch
    {
        public static void Postfix(MeetingHud __instance)
        {
            __instance.gameObject.AddComponent<MeetingHudBehaviour>().meetingHud = __instance;
        }
    }

}