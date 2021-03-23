using HarmonyLib;
using BetterUI.Patches;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterCookingStation
  {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CookingStation), "GetHoverText")]
    private static bool GetHoverText(CookingStation __instance, ref string __result)
    {
      if (Main.timeLeftStyleCookingStation.Value == 0) return true;

      return Patches.HoverText.PatchCookingStation(__instance, ref __result);
    }
  }
}
