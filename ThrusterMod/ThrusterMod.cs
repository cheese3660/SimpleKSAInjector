using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using SimpleKSAInjector;

namespace ThrusterMod;


[HarmonyPatch]
public class ThrusterMod
{
    [ModPostEntryPoint]
    public static void SetupEverything()
    {
        var harmony = new Harmony("ThrusterMod");
        harmony.PatchAll();
    }
}