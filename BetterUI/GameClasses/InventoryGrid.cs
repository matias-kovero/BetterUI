using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterInventoryGrid
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(InventoryGrid), "UpdateGui")]
    private static void PatchInventory(ref InventoryGrid __instance, ref Player player, ItemDrop.ItemData dragItem)
    {
      if (!Main.showDurabilityColor.Value && !Main.showItemStars.Value) return;

      foreach (ItemDrop.ItemData itemData in __instance.m_inventory.GetAllItems())
      {
        InventoryGrid.Element element = __instance.GetElement(itemData.m_gridPos.x, itemData.m_gridPos.y, __instance.m_inventory.GetWidth());
        if (element.m_icon.transform.localScale == Vector3.one) element.m_icon.transform.localScale *= Mathf.Max(Main.iconScaleSize.Value, 0.1f);
        if (itemData.m_shared.m_useDurability)
        {
          if (itemData.m_durability <= 0f) // Item has no durability, original code should do this
          {
            //element.m_durability.SetValue(1f);
            //element.m_durability.SetColor((Mathf.Sin(Time.time * 10f) > 0f) ? Color.red : new Color(0f, 0f, 0f, 0f));

          }
          else // Item has durability left
          {
            if (Main.showDurabilityColor.Value)
            {
              Patches.DurabilityBar.UpdateColor(element, itemData.GetDurabilityPercentage());
            }
          }
        }
        // Change item quality info
        if (itemData.m_shared.m_maxQuality > 1)
        {
          if (Main.showItemStars.Value)
          {
            Patches.Stars.Draw(element, itemData.m_quality);
          }
        }
      }
    }
  }
}
