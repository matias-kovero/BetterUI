using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterItemDrop
  {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemDrop), "GetHoverText")]
    private static bool PatchHoverText(ref ItemDrop __instance, ref string __result)
    {
      string text = __instance.m_itemData.m_shared.m_name;
      if (__instance.m_itemData.m_quality > 1)
      {
        text = string.Concat(new object[]
        {
          text,
          " (<color=yellow>",
          Patches.Stars.HoverText(__instance.m_itemData.m_quality),
          "</color>) "
        });
      }
      if (__instance.m_itemData.m_stack > 1)
      {
        text = text + " x" + __instance.m_itemData.m_stack.ToString();
      }
      __result = Localization.instance.Localize(text + "\n[<color=yellow><b>$KEY_Use</b></color>] $inventory_pickup");
      return false;
    }

    [HarmonyPatch]
    static class ItemData
    {
      [HarmonyPrefix]
      [HarmonyPatch(typeof(ItemDrop.ItemData), "GetTooltip", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(bool) })]
      public static bool PatchTooltip(ref string __result, ItemDrop.ItemData item, int qualityLevel, bool crafting)
      {
        if (!Main.showCustomTooltips.Value) return true;
        __result = Patches.BetterTooltip.Create(item, qualityLevel, crafting);
        return false; // https://harmony.pardeike.net/articles/patching-prefix.html#changing-the-result-and-skipping-the-original
      }
    }
  }
}
