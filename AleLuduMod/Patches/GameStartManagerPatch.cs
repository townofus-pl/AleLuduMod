using HarmonyLib;

namespace AleLuduMod.Patches;

internal static class GameStartManagerPatch
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameStartManagerUpdatePatch
    {
        public static void Postfix(GameStartManager __instance)
        {
            if (GameData.Instance == null || AmongUsClient.Instance == null || GameManager.Instance == null || GameManager.Instance.LogicOptions == null || __instance.PlayerCounter == null) return;

            int playerCount = GameData.Instance.PlayerCount;
            int minPlayers = __instance.MinPlayers;

            string counterColor = playerCount switch
            {
                _ when playerCount < minPlayers => "<color=#FF0000FF>",
                _ when playerCount == minPlayers => "<color=#FFFF00FF>",
                _ => "<color=#00FF00FF>"
            };

            int maxPlayers = AmongUsClient.Instance.NetworkMode is NetworkModes.LocalGame ? AleLuduModPlugin.MaxPlayers : GameManager.Instance.LogicOptions.MaxPlayers;

            __instance.PlayerCounter.text = $"{counterColor}{playerCount}/{maxPlayers}";
        }
    }
}