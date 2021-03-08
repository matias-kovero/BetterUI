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
  static class BetterHotkeyBar
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(HotkeyBar), "UpdateIcons")]
    private static void PatchHotkeyBar(ref HotkeyBar __instance, ref Player player)
    {
      if (!player || player.IsDead())
      {
        return;
      }

      if (!Main.showDurabilityColor.Value) return;

      foreach (ItemDrop.ItemData itemData in __instance.m_items)
      {
        HotkeyBar.ElementData element = __instance.m_elements[itemData.m_gridPos.x];

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
            Patches.DurabilityBar.UpdateColor(element, itemData.GetDurabilityPercentage());
          }
        }
      }
    }
  }
}
