using HarmonyLib;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace AleLuduMod.Patches
{
    [HarmonyPatch]
    class OptionsPatches
    {
        private static GameObject? popUp;
        private static TextMeshPro? titleText;
        private static ToggleButtonBehaviour? buttonPrefab;
        private static Vector3? _origin;

        [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
        [HarmonyPostfix]

        public static void OptionsMenuBehaviour_StartPostfix(OptionsMenuBehaviour __instance)
        {
            if (!__instance.CensorChatButton) return;

            
            if (!buttonPrefab)
            {
                buttonPrefab = Object.Instantiate(__instance.CensorChatButton);
                Object.DontDestroyOnLoad(buttonPrefab);
                buttonPrefab.name = "CensorChatPrefab";
                buttonPrefab.gameObject.SetActive(false);
            }
            if (!popUp)
            {
                CreateOptionsPanel(__instance);
            }
            InitializeMoreButton(__instance);
        }

        private static void CreateOptionsPanel(OptionsMenuBehaviour prefab)
        {
            popUp = Object.Instantiate(prefab.gameObject);
            Object.DontDestroyOnLoad(popUp);
            var transform = popUp.transform;
            var pos = transform.localPosition;
            pos.z = -810f;
            transform.localPosition = pos;

            Object.Destroy(popUp.GetComponent<OptionsMenuBehaviour>());

            
            for (var i = 0; i < popUp.transform.childCount; i++)
            {
                var gObj = popUp.transform.GetChild(i).gameObject;
                if (gObj.name != "Background" && gObj.name != "CloseButton")
                    Object.Destroy(gObj);
            }

            popUp.SetActive(false);
        }

        private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
        {
            var moreOptions = Object.Instantiate(buttonPrefab, __instance.CensorChatButton.transform.parent);
            var transform = __instance.CensorChatButton.transform;
            _origin ??= transform.localPosition;

            moreOptions.transform.localPosition = _origin.Value + new Vector3(3.8f,-1.15f,0);
            moreOptions.transform.localScale = new Vector3(0.4f, 0.75f, 1);

            moreOptions.gameObject.SetActive(true);
            moreOptions.Text.text = "AleLudu Options...";
            moreOptions.Text.transform.localScale = new Vector3(.6f / 0.4f, 0.9f, 1);
            var moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
            moreOptionsButton.OnClick = new ButtonClickedEvent();
            moreOptionsButton.OnClick.AddListener((Action)(() =>
            {
                bool closeUnderlying = false;
                if (!popUp) return;

                if (__instance.transform.parent && __instance.transform.parent == HudManager.Instance.transform)
                {
                    popUp.transform.SetParent(HudManager.Instance.transform);
                    popUp.transform.localPosition = new Vector3(0, 0, -800f);
                    closeUnderlying = true;
                }
                else
                {
                    popUp.transform.SetParent(null);
                    Object.DontDestroyOnLoad(popUp);
                }

                refreshOptionsView();

                if (closeUnderlying)
                    __instance.Close();
            }));
        }

        private static void refreshOptionsView()
        {
            popUp.gameObject.SetActive(false);
            popUp.gameObject.SetActive(true);

            if (popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>()) return;
            //setup title
            var title = Object.Instantiate(buttonPrefab, popUp.transform);
            title.transform.localPosition = new Vector3(0f, 2.2f, -.5f);
            title.Background.color = Palette.White;
            title.onState = true;
            title.Text.text = "AleLuduMod Options";
            title.Text.fontSizeMin = title.Text.fontSizeMax = 1.4f;

            foreach (var spr in title.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                spr.size = new Vector2(3.5f, .45f);
            }
            title.transform.localScale *= 1.7f;
            title.gameObject.SetActive(true);

            var titleButton = title.GetComponent<PassiveButton>();
            titleButton.OnClick = new ButtonClickedEvent();
            titleButton.OnMouseOut = new UnityEvent();
            titleButton.OnMouseOver = new UnityEvent();

            titleButton.OnClick.AddListener((Action)(() =>
            {
                title.Background.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                title.onState = !title.onState;
                title.Text.text = title.onState ? "AleLuduMod Options" : "townofus.pl";
            }));

            //setup options
            var button = Object.Instantiate(buttonPrefab, popUp.transform);

            button.transform.localPosition = new Vector3(-1.17f, 1.3f, -.5f);

            button.onState = AleLuduModPlugin.Force4Columns.Value;
            button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
            button.Text.text = "Always show 4 columns";
            button.Text.fontSizeMin = button.Text.fontSizeMax = 1.8f;

            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                spr.size = new Vector2(2.2f, .7f);
            }

            button.gameObject.SetActive(true);

            var passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOver = new UnityEvent();

            passiveButton.OnClick.AddListener((Action)(() =>
            {
                button.onState = AleLuduModPlugin.Force4Columns.Value = !AleLuduModPlugin.Force4Columns.Value;
                button.Background.color = button.onState ? Color.green : Palette.ImpostorRed;
            }));
            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, 255)));
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? Color.green : Palette.ImpostorRed));
        }
    }
}

