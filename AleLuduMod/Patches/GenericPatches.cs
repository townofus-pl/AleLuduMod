using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using InnerNet;

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
}