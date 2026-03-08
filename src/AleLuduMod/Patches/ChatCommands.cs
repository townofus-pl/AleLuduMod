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
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, ref string chatText)
            {
                if (__instance != HudManager.Instance.Chat) return true;

                // After entering the command, when you try to join the lobby it will show "X/15". Only after the game is over will there be a larger lobby.
                if (chatText.StartsWith("!limit "))
                {
                    if ((AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started || AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) && sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            chatText = "You cannot use this command during the game!";
                            error = true;
                        }
                        return sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                    }
                    if (GameData.Instance.GetHost() == sourcePlayer.Data && sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
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
                                        chatText = $"Player limit has been set for: <color=#D91919FF><b>{newLimit}</b></color>";
                                        system = true;
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                {
                                    chatText = "The !limit command has a range of 4 - 35!";
                                    error = true;
                                }
                            }
                        }
                        else
                        {
                            if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                            {
                                chatText = "Use !limit [number]. Example: !limit 20";
                                error = true;
                            }
                        }
                    }
                    else if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        if (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            chatText = "You don't have access to this command!";
                            noaccess = true;
                        }
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