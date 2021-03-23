using HarmonyLib;

namespace BetterUI.GameClasses
{
  //[HarmonyPatch]
  static class BetterSmelter
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Smelter), "UpdateHoverTexts")]
    private static void UpdateHoverTexts(Smelter __instance)
    {
      // Unimplemented
      //Patches.HoverText.PatchSmelter(__instance);
    }
  }
}
