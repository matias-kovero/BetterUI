using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BetterUI.Patches;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  public static class BetterInventoryGui
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(InventoryGui), "UpdateCharacterStats")]
    public static void PatchArmor(ref InventoryGui __instance, ref Player player)
    {
      if (!Main.showCombinedItemStats.Value) return;
      if (InventoryArmorTooltip.tooltip == null)
      {
        InventoryArmorTooltip.Awake(__instance);
      }
      float bodyArmor = player.GetBodyArmor();
      InventoryArmorTooltip.m_armor.text = bodyArmor.ToString();

      InventoryArmorTooltip.Update(player);
    }
  }
}
