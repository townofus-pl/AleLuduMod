using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime;
using InnerNet;

namespace AleLuduMod.Patches;

internal static class GenericPatches
{
    [HarmonyPatch(typeof(InnerNetServer), nameof(InnerNetServer.HandleNewGameJoin))]
    public static class InnerNetServer_HandleNewGameJoin
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

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class RemoveVanillaServerPatch
    {
        public static void Postfix() => Utils.RemoveVanillaServer();
    }
}