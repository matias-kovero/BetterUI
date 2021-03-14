using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterContainer
  {
    private static readonly string openContainer = "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_container_open";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Container), "GetHoverText")]
    private static void GetHoverText(Container __instance, ref string __result)
    {
      if (Main.chestHasRoomStyle.Value == 0) return;
      if (__instance.m_inventory.NrOfItems() != 0)
      {
        string room = Main.chestHasRoomStyle.Value == 1 ? $"{__instance.m_inventory.SlotsUsedPercentage()}%" : $"{__instance.m_inventory.NrOfItems()}/{__instance.m_inventory.GetWidth()* __instance.m_inventory.GetHeight()}";
        __result = Localization.instance.Localize($"{__instance.m_name}: {room} {openContainer}");
      }
    }
  }
}
