using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Reflection;
using InnerNet;
using System.Linq;

namespace AleLuduMod.Patches;

internal static class GenericPatches
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameStartManagerUpdatePatch
    {
        private static string fixDummyCounterColor = string.Empty;

        public static void Prefix(GameStartManager __instance)
        {
            if (GameData.Instance == null || __instance.LastPlayerCount == GameData.Instance.PlayerCount) return;

            if (__instance.LastPlayerCount > __instance.MinPlayers)
            {
                fixDummyCounterColor = "<color=#00FF00FF>";
            }
            else if (__instance.LastPlayerCount == __instance.MinPlayers)
            {
                fixDummyCounterColor = "<color=#FFFF00FF>";
            }
            else
            {
                fixDummyCounterColor = "<color=#FF0000FF>";
            }
        }

        public static void Postfix(GameStartManager __instance)
        {
            if (GameData.Instance == null || AmongUsClient.Instance == null || GameManager.Instance == null || GameManager.Instance.LogicOptions == null || string.IsNullOrEmpty(fixDummyCounterColor)) return;

            int maxPlayersNum = AmongUsClient.Instance.NetworkMode is NetworkModes.LocalGame ? AleLuduModPlugin.MaxPlayers : GameManager.Instance.LogicOptions.MaxPlayers;
            __instance.PlayerCounter.text = $"{fixDummyCounterColor}{GameData.Instance.PlayerCount}/{maxPlayersNum}";
            fixDummyCounterColor = string.Empty;
        }
    }

    [HarmonyPatch(typeof(InnerNetServer), nameof(InnerNetServer.HandleNewGameJoin))]
    public static class InnerNetSerer_HandleNewGameJoin
    {
        public static bool Prefix(InnerNetServer __instance, [HarmonyArgument(0)] InnerNetServer.Player client)
        {
            if (__instance.Clients.Count is < 15 or >= AleLuduModPlugin.MaxPlayers) return true;

            __instance.Clients.Add(client);

            client.LimboState = LimboStates.PreSpawn;
            if (__instance.HostId == -1)
            {
                __instance.HostId = __instance.Clients.ToArray()[0].Id;
            }

            if (__instance.HostId == client.Id)
            {
                client.LimboState = LimboStates.NotLimbo;
            }

            var writer = MessageWriter.Get(SendOption.Reliable);
            try
            {
                __instance.WriteJoinedMessage(client, writer, true);
                client.Connection.Send(writer);
                __instance.BroadcastJoinMessage(client, writer);
            }
            catch (Il2CppException exception)
            {
                UnityEngine.Debug.LogError("[CM] InnerNetServer::HandleNewGameJoin MessageWriter 2 Exception: " +
                               exception.Message);
                // Debug.LogException(exception, __instance);
            }
            finally
            {
                writer.Recycle();
            }

            return false;
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
                impostorsOption.ValidRange.max = AleLuduModPlugin.MaxPlayers / 2;
            }
        }
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