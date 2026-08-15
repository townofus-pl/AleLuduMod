using BepInEx.Unity.IL2CPP;

namespace AleLuduMod;

public static class ModCompatibility
{
    public static void Loaded()
    {
        IL2CPPChainloader.Instance.Finished += MiraApiCompatibility.Initialize;
        IL2CPPChainloader.Instance.Finished += TheOtherRolesCompatibility.Initialize;
        IL2CPPChainloader.Instance.Finished += StellarRolesCompatibility.Initialize;
        IL2CPPChainloader.Instance.Finished += AllTheRolesCompatibility.Initialize;
    }
}
