using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
  [HarmonyPatch]
  public static class RPG
  {
    private static Player RPG_Player = null;
    private static GuiBar XP_Bar = null;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Hud), "Awake")]
    private static void InitializeXPBar(ref Hud __instance)
    {
      if (!Main.showCharacterXP.Value) return;
      if (XP_Bar == null)
      {
        XP_Bar = XPBar.Awake(__instance);
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Hud), "Update")]
    private static void UpdateXPBar(Hud __instance)
    {
      Player localPlayer = Player.m_localPlayer;
      if (!Main.showCharacterXP.Value) return;
      if (RPG_Player == null && localPlayer != null)
      {
        RPG_Player = localPlayer;
        XP.Awake(localPlayer);
        XP_Bar.SetValue(XP.GetLevelPercentage());
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Skills.Skill), "Raise")]
    private static void CalculateXP(ref Skills __instance, float factor)
    {
      if (XP_Bar == null) return;

      XP.RaiseXP();
      XP_Bar.SetValue(XP.GetLevelPercentage());
    }
  }

  [HarmonyPatch]
  public static class Items
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(InventoryGrid), "UpdateGui")]
    private static void PatchInventory(ref InventoryGrid __instance, ref Player player, ItemDrop.ItemData dragItem)
    {
      if (!Main.showDurabilityColor.Value) return;

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
            DurabilityBar.UpdateColor(element, itemData.GetDurabilityPercentage());
          }
        }
        // Change item quality info
        if (itemData.m_shared.m_maxQuality > 1)
        {
          Stars.Draw(element, itemData.m_quality);
        }
      }
    }

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
            DurabilityBar.UpdateColor(element, itemData.GetDurabilityPercentage());
          }
        }
      }
    }

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
          Helpers.Repeat("\u2605", __instance.m_itemData.m_quality),
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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ItemDrop.ItemData), "GetTooltip", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(bool) })]
    public static bool PatchTooltip(ref string __result, ItemDrop.ItemData item, int qualityLevel, bool crafting)
    {
      return true;
      __result = CustomTooltip.ChangeTooltip(item, qualityLevel, crafting);
      return false; // https://harmony.pardeike.net/articles/patching-prefix.html#changing-the-result-and-skipping-the-original
    }
  }
  [HarmonyPatch]
  public static class Skill
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Skills), "RaiseSkill")]
    private static void Notification(ref Skills __instance, ref Skills.SkillType skillType, ref float factor)
    {
      if (Main.showXPNotifications.Value)
      {
        Skills.Skill skill = __instance.GetSkill(skillType);
        XPNotification.Show(skill, factor);
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SkillsDialog), "Setup")]
    private static void UpdateGUI(ref SkillsDialog __instance, ref Player player)
    {
      __instance.gameObject.SetActive(true);
      if (Main.showCharacterXP.Value)
      {
        Utils.FindChild(__instance.transform, "topic").GetComponent<Text>().text = $"LV. {XP.level:0}";
      }
      if (!Main.customSkillUI.Value) return;

      SkillUI.UpdateDialog(__instance, player);
    }
  }

  [HarmonyPatch]
  public static class Menu
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FejdStartup), "SetupGui")]
    private static void AddWaterMark(ref FejdStartup __instance)
    {
      CustomWatermark.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FejdStartup), "UpdateCharacterList")]
    private static void ShowCharacterStats(ref FejdStartup __instance)
    {
      if (Main.showCustomCharInfo.Value)
      {
        CharacterStats.Show(__instance);
      }
    }
  }

  [HarmonyPatch]
  public static class HudEnemy
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyHud), "Awake")]
    private static void PatchDefaults(ref EnemyHud __instance)
    {
      __instance.m_maxShowDistance = Mathf.Min(Mathf.Abs(Main.maxShowDistance.Value), CustomEnemyHud.maxDrawDistance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyHud), "ShowHud")]
    private static void PatchName(ref EnemyHud __instance, ref Character c)
    {
      if (!Main.customEnemyHud.Value) return;
      CustomEnemyHud.AddLvlToName(__instance, c);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyHud), "UpdateHuds")]
    private static void UpdateHP(ref EnemyHud __instance, Player player, float dt)
    {
      if (!Main.customEnemyHud.Value) return;
      CustomEnemyHud.UpdateHPText(__instance);
    }
  }
}
