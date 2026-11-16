using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AleLuduMod.Patches;

[HarmonyPatch]
internal static class EnterCodeManagerPatch
{
    private static GameObject? EnterCodeSelectServer;
    public static MainMenuManager? MainMenuManager = null;

    [HarmonyPatch(typeof(EnterCodeManager), nameof(EnterCodeManager.OnEnable))]
    [HarmonyPostfix]
    public static void OnEnable(EnterCodeManager __instance)
    {
        if (TheOtherRolesCompatibility.TheOtherRolesIsLoaded) return;

        var mainmenu = MainMenuManager;
        if (mainmenu != null)
        {
            ServerDropdown? dropdown = null;
            if (!EnterCodeSelectServer)
            {
                var obj = UnityEngine.Object.Instantiate(mainmenu.createGameScreen.serverButton.transform.parent.gameObject, __instance.enterCodeField.transform.parent);
                obj.name = "EnterCodeSelectServer";
                obj.transform.localPosition = new(2.8596f, 1.234f);
                dropdown = UnityEngine.Object.Instantiate(mainmenu.createGameScreen.serverDropdown, obj.transform);
                EnterCodeSelectServer = obj;
            }
            else
            {
                dropdown = EnterCodeSelectServer.GetComponentInChildren<ServerDropdown>();
            }
            mainmenu.entercodeField.transform.parent.localPosition = new(-1.1106f, 0.12f);
            List<TextMeshPro> tmps = new();
            EnterCodeSelectServer.transform.Find("BlackSquare").transform.localPosition = new(0.554f, 0.658f, -1f);
            GameObject serverButton = EnterCodeSelectServer.transform.Find("ServerBox").gameObject;
            serverButton.transform.localPosition = new(-0.97f, 0f);
            tmps.Add(serverButton.transform.Find("Inactive/ClassicText").GetComponent<TextMeshPro>());
            tmps.Add(serverButton.transform.Find("Highlight/ClassicText").GetComponent<TextMeshPro>());
            void UpdateServer(string text)
            {
                tmps.Do(t => t.text = text);
            }
            string? SetCurrentServer()
            {
                var serverManager = ServerManager.Instance;
                var translationController = TranslationController.Instance;
                IRegionInfo currentRegion = serverManager.CurrentRegion;

                if (currentRegion == null || translationController == null)
                {
                    Error("The server cannot be configured: ServerManager, CurrentRegion, or TranslationController is missing.");
                    return null;
                }

                return translationController.GetStringWithDefault(currentRegion.TranslateName, currentRegion.Name, new Il2CppReferenceArray<Il2CppSystem.Object>(Array.Empty<Il2CppSystem.Object>()));
            }
            void OpenServerDropdown()
            {
                dropdown?.gameObject.SetActive(true);
                serverButton.SetActive(false);
            }
            void CloseServerDropdown()
            {
                serverButton.SetActive(true);
            }
            var serverName = SetCurrentServer();
            if (serverName != null)
            {
                UpdateServer(serverName);
            }
            dropdown.transform.localPosition = new(0.5348f, -1.2209f, -15f);
            dropdown.transform.localScale = Vector2.one;
            dropdown.Initialize(new Action<string>(s => UpdateServer(s)), new Action(() => CloseServerDropdown()));
            var button = EnterCodeSelectServer.transform.Find("ServerBox").GetComponent<PassiveButton>();
            button.OnClick = new();
            button.OnClick.AddListener((Action)(() =>
            {
                OpenServerDropdown();
            }));
            dropdown.transform.GetChild(0).GetComponent<PassiveButton>().enabled = false;
        }
    }
}