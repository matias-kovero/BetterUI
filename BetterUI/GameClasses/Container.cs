using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterContainer
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Container), "GetHoverText")]
    private static void GetHoverText(Container __instance, ref string __result)
    {
      if (Main.chestHasRoomStyle.Value == 0) return;

      if (__instance.m_inventory.NrOfItems() != 0)
      {
        __result = Patches.HoverText.PatchContainer(__instance);
      }
    }
  }
}
