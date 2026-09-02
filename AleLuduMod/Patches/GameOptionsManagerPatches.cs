using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;
using System.Linq;

namespace AleLuduMod.Patches;

internal static class GameOptionsManagerPatches
{
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
                Error($"Failed to adjust options recommendations: {e}");
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
                Error($"Failed to adjust options recommendations: {e}");
            }
        }
    }

    private static void TryAdjustOptionsRecommendations(GameOptionsManager? manager)
    {
        if (manager == null)
        {
            Error("GameOptionsManager was null! Cannot set recommendations!");
            return;
        }

        const int maxPlayers = AleLuduModPlugin.MaxPlayers;
        var options = manager.GameHostOptions.Cast<Il2CppSystem.Object>();
        if (options == null)
        {
            Error("GameHostOptions was null! Cannot set recommendations!");
            return;
        }

        var type = options.GetIl2CppType();

        var maxRecommendation = ((Il2CppStructArray<int>)Enumerable.Repeat(maxPlayers, maxPlayers + 1).ToArray()).Cast<Il2CppSystem.Object>();
        var minRecommendation = ((Il2CppStructArray<int>)Enumerable.Repeat(4, maxPlayers + 1).ToArray()).Cast<Il2CppSystem.Object>();
        var killRecommendation = ((Il2CppStructArray<int>)Enumerable.Repeat(0, maxPlayers + 1).ToArray()).Cast<Il2CppSystem.Object>();

        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        // all these fields are currently static, but we're doing a forward compat
        // static fields ignore object param so non-null instance is ok
        type.GetField("RecommendedImpostors", flags)?.SetValue(options, maxRecommendation); // Doesn't exist on HnS options
        type.GetField("MaxImpostors", flags)?.SetValue(options, maxRecommendation);
        type.GetField("RecommendedKillCooldown", flags)?.SetValue(options, killRecommendation);
        type.GetField("MinPlayers", flags)?.SetValue(options, minRecommendation);

        Info("Adjusted options recommendations");
    }
}
