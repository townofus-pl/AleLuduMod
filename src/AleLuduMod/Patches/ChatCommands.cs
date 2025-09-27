using AmongUs.GameOptions;
using HarmonyLib;

namespace AleLuduMod.Patches
{
    public class ChatCommands
    {
        public static bool system = false;
        public static bool error = false;
        public static bool noaccess = false;

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public class Commands
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer , ref string chatText)
            {
                if (__instance != HudManager.Instance.Chat) return true;

                // After entering the command, when you try to join the lobby it will show "X/15". Only after the game is over will there be a larger lobby.
                if (chatText.StartsWith("!limit "))
                {
                    if (GameData.Instance.GetHost() == sourcePlayer.Data)
                    {
                        string[] args = chatText.Split(' ');
                        if (args.Length > 1 && int.TryParse(args[1], out int newLimit))
                        {
                            if (newLimit >= 4 && newLimit <= 35)
                            {
                                try
                                {
                                    GameOptionsManager.Instance.CurrentGameOptions.SetInt(Int32OptionNames.MaxPlayers, newLimit);
                                    if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                    {
                                        system = true;
                                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"A player limit has been set for: <color=#D91919FF>{newLimit}</color>");
                                    }
                                    return false;
                                }
                                catch { }
                            }
                            else
                            {
                                if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                {
                                    error = true;
                                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "The limit must be between 4 and 35.");
                                }
                                return false;
                            }
                        }
                        else
                        {
                            if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                            {
                                error = true;
                                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Use !limit [number]. Example: !limit 20");
                            }
                            return false;
                        }
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            noaccess = true;
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You don't have access to this command!");
                        }
                        return false;
                    }
                    return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
        public class ChatFixName
        {
            public static bool Prefix(ChatBubble __instance)
            {
                if (error)
                {
                    __instance.NameText.text = "ERROR";
                    __instance.NameText.color = Palette.ImpostorRed;
                    __instance.NameText.ForceMeshUpdate(true, true);
                    __instance.Xmark.enabled = true;
                    __instance.votedMark.enabled = false;
                    error = false;
                    return false;
                }
                else if (system)
                {
                    __instance.NameText.text = "SYSTEM MESSAGE";
                    __instance.NameText.color = Palette.CrewmateBlue;
                    __instance.NameText.ForceMeshUpdate(true, true);
                    __instance.Xmark.enabled = false;
                    __instance.votedMark.enabled = false;
                    system = false;
                    return false;
                }
                else if (noaccess)
                {
                    __instance.NameText.text = "NO ACCESS";
                    __instance.NameText.color = Palette.Blue;
                    __instance.NameText.ForceMeshUpdate(true, true);
                    __instance.Xmark.enabled = true;
                    __instance.votedMark.enabled = false;
                    noaccess = false;
                    return false;
                }
                else return true;
            }
        }
    }
}
