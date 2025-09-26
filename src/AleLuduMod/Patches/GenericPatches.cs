using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;
using Reactor.Utilities;
using System.Linq;

namespace AleLuduMod.Patches;

internal static class GenericPatches
{
    // I did not find a use of this method, but still patching for future updates
    // maxExpectedPlayers is unknown, looks like server code tbh
    [HarmonyPatch(typeof(LegacyGameOptions), nameof(LegacyGameOptions.AreInvalid))]
    public static class InvalidOptionsPatches
    {
        public static bool Prefix(LegacyGameOptions __instance, [HarmonyArgument(0)] int maxExpectedPlayers)
        {
            return __instance.MaxPlayers > maxExpectedPlayers ||
                   __instance.NumImpostors < 1 ||
                   __instance.NumImpostors + 1 > maxExpectedPlayers / 2 ||
                   __instance.KillDistance is < 0 or > 2 ||
                   __instance.PlayerSpeedMod is <= 0f or > 3f;
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameStartManagerUpdatePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CreateGameOptions), nameof(CreateGameOptions.Show))]
        public static void CreateGameOptionsShowPostfix(CreateGameOptions __instance)
        {
            var numberOption = __instance.gameObject.GetComponentInChildren<NumberOption>(true);
            if (numberOption != null)
            {
                numberOption.ValidRange.max = AleLuduModPlugin.MaxPlayers;
            }
        }
    }

    private static void TryAdjustOptionsRecommendations(GameOptionsManager manager)
    {
        const int MaxPlayers = AleLuduModPlugin.MaxPlayers;
        var type = manager.GetGameOptions();
        var options = manager.GameHostOptions.Cast<Il2CppSystem.Object>();

        var maxRecommendation = ((Il2CppStructArray<int>)Enumerable.Repeat(MaxPlayers, MaxPlayers + 1).ToArray())
            .Cast<Il2CppSystem.Object>();
        var minRecommendation = ((Il2CppStructArray<int>)Enumerable.Repeat(4, MaxPlayers + 1).ToArray())
            .Cast<Il2CppSystem.Object>();
        var killRecommendation = ((Il2CppStructArray<int>)Enumerable.Repeat(0, MaxPlayers + 1).ToArray())
            .Cast<Il2CppSystem.Object>();


        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        // all these fields are currently static, but we're doing a forward compat
        // static fields ignore object param so non-null instance is ok
        type.GetField("RecommendedImpostors", flags)?.SetValue(options, maxRecommendation);
        type.GetField("MaxImpostors", flags)?.SetValue(options, maxRecommendation);
        type.GetField("RecommendedKillCooldown", flags)?.SetValue(options, killRecommendation);
        type.GetField("MinPlayers", flags)?.SetValue(options, minRecommendation);
    }

    [HarmonyPatch(typeof(GameOptionsManager), nameof(GameOptionsManager.GameHostOptions), MethodType.Setter)]
    public static class GameOptionsManager_set_GameHostOptions
    {
        public static void Postfix(GameOptionsManager __instance)
        {
            try
            {
                TryAdjustOptionsRecommendations(__instance);
            }
            catch (System.Exception e)
            {
                Logger<AleLuduModPlugin>.Error($"Failed to adjust options recommendations: {e}");
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsManager), nameof(GameOptionsManager.SwitchGameMode))]
    public static class GameOptionsManager_SwitchGameMode
    {
        public static void Postfix(GameOptionsManager __instance)
        {
            try
            {
                TryAdjustOptionsRecommendations(__instance);
            }
            catch (System.Exception e)
            {
                Logger<AleLuduModPlugin>.Error($"Failed to adjust options recommendations: {e}");
            }
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
