

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;

namespace AleLuduMod;

public static class ModCompatibility
{
    public const string MiraApiGuid = "mira.api";
    public static bool MiraApiLoaded { get; private set; }
    private static BasePlugin MiraApiPlugin { get; set; }
    private static Assembly MiraApiAssembly { get; set; }
    private static Type[] MiraApiTypes { get; set; }
    public static float XStart = -0.8f;
    public static float YStart = 2.15f;
    public static float XOffset = 1.95f;
    public static float YOffset = -0.65f;

    public static void Initialize()
    {
        InitMiraApi();
    }

    private static void InitMiraApi()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(MiraApiGuid, out var value))
        {
            return;
        }

        MiraApiPlugin = (value.Instance as BasePlugin)!;
        MiraApiAssembly = MiraApiPlugin.GetType().Assembly;

        MiraApiTypes = AccessTools.GetTypesFromAssembly(MiraApiAssembly);
        
        var playerMenu = MiraApiTypes.First(t => t.Name == "CustomPlayerMenu");
        var menuBegin = AccessTools.Method(playerMenu, "Begin", new [] {typeof(Func<PlayerControl, bool>), typeof(Action<PlayerControl?>)});

        var compatType = typeof(ModCompatibility);
        var harmony = new Harmony("aleludu.miraapi.patch");
        harmony.Patch(menuBegin, null,
            new HarmonyMethod(AccessTools.Method(compatType, nameof(BeginPostfix))));

        MiraApiLoaded = true;
        Logger<AleLuduModPlugin>.Message("MiraAPI was detected and patched");
    }
    public static void BeginPostfix(dynamic __instance, Func<PlayerControl, bool> playerMatch, Action<PlayerControl?> onClick)
    {
        var targets = __instance.potentialVictims as List<ShapeshifterPanel>;
        if (targets == null || targets.Count() < 16 && !AleLuduModPlugin.Force4Columns.Value)
        {
            // dont change layout if players count is below 16
            return;
        }

        var i = 0;

        foreach (var panel in targets)
        {
            panel.gameObject.SetActive(true);

            var row = i / 4;
            var col = i % 4;
            var buttonTransform = panel.transform;
            buttonTransform.localScale *= 0.75f;
            buttonTransform.localPosition = new Vector3(
                XStart + XOffset * col * 1.1f - 2.4f,
                YStart + YOffset * row * 0.9255f - 0.65f,
                buttonTransform.localPosition.z
            );
            i++;
        }
    }
}