using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace AleLuduMod.Modules;

public static class SyzyfowyTouCompatibility
{
    public const string SyzyfowyTouGuid = "com.syzyf.townofus";
    public static bool SyzyfowyTouIsLoaded { get; private set; }
    private static BasePlugin TownOfUs { get; set; }
    private static Assembly SyzyfowyTouAssembly { get; set; }
    private static Type[] SyzyfowyTouTypes { get; set; }

    public static void Initialize() => InitSyzyfowyTou();

    private static void InitSyzyfowyTou()
    {
        if (!IL2CPPChainloader.Instance.Plugins.TryGetValue(SyzyfowyTouGuid, out var value)) return;

        TownOfUs = (value.Instance as BasePlugin)!;
        SyzyfowyTouAssembly = TownOfUs.GetType().Assembly;

        SyzyfowyTouTypes = AccessTools.GetTypesFromAssembly(SyzyfowyTouAssembly);

        var playerMenu = SyzyfowyTouTypes.First(t => t.Name == "CustomPlayerMenu");
        var menuBegin = playerMenu.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(method =>
            {
                return method.Name == "Begin";
            });
        if (menuBegin.Count() == 0)
        {
            Error("Could not find any compatible CustomPlayerMenu.Begin method");
            return;
        }

        var harmony = new Harmony("aleludu.syzyfowytou.patch");
        foreach (var type in menuBegin)
        {
            harmony.Patch(type, null, new HarmonyMethod(AccessTools.Method(typeof(MiraApiCompatibility), nameof(MiraApiCompatibility.BeginPostfix))));
        }

        SyzyfowyTouIsLoaded = true;
        Message("Syzyfowy Town Of Us has successfully loaded!");
    }
}