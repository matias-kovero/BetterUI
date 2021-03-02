using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
  static class DurabilityBar
  {
    private static Color[] normal = new Color[]
    {
      new Color(0.11765f, 0.72941f, 0.03529f, 1f), // Green
      new Color(0.72941f, 0.72941f, 0.03529f, 1f), // Yellow
      new Color(0.72941f, 0.34902f, 0.03529f, 1f), // Orange
      new Color(0.72941f, 0.03529f, 0.03529f, 1f)  // Red
    };

    public static void UpdateColor(InventoryGrid.Element element, float durability)
    {
      element.m_durability.SetValue(durability);
      // Might be to update items colorbar? This is from original code.
      element.m_durability.ResetColor();
      // Between 1f - 0f
      switch (durability)
      {
        case float n when (n >= 0.75f):
          // Color green
          element.m_durability.SetColor(normal[0]);
          break;
        case float n when (n >= 0.50f):
          // Color yellow
          element.m_durability.SetColor(normal[1]);
          break;
        case float n when (n >= 0.25f):
          // Color Orange
          element.m_durability.SetColor(normal[2]);
          break;
        case float n when (n >= 0f):
          // Color Red
          element.m_durability.SetColor(normal[3]);
          break;
      }
    }

    public static void UpdateColor(HotkeyBar.ElementData element, float durability)
    {
      element.m_durability.SetValue(durability);
      // Might be to update items colorbar? This is from original code.
      element.m_durability.ResetColor();
      // Between 1f - 0f
      switch (durability)
      {
        case float n when (n >= 0.75f):
          // Color green
          element.m_durability.SetColor(normal[0]);
          break;
        case float n when (n >= 0.50f):
          // Color yellow
          element.m_durability.SetColor(normal[1]);
          break;
        case float n when (n >= 0.25f):
          // Color Orange
          element.m_durability.SetColor(normal[2]);
          break;
        case float n when (n >= 0f):
          // Color Red
          element.m_durability.SetColor(normal[3]);
          break;
      }
    }
  }

  static class Stars
  {
    private static Color starColor = new Color(1.0f, 0.85882f, 0.23137f, 1.0f);

    public static void Draw(InventoryGrid.Element element, int quality_lvl)
    {
      string stars;
      if(quality_lvl >= 5) // Fix for modded items...
      {
        stars = $"<color=white><size=10>{quality_lvl}</size>x </color>\u2605"; // Could be ★x5 or ★x99999
        element.m_quality.alignment = TextAnchor.MiddleRight;
        element.m_quality.rectTransform.sizeDelta = new Vector2(60f, 20f);
        if (quality_lvl > 99998) stars = $"<color=white><size=10>MODDED</size> </color>\u2605"; // For them wack guys
      } else
      {
        stars = Helpers.Repeat("\u2605", quality_lvl);
        element.m_quality.rectTransform.sizeDelta = new Vector2(quality_lvl * 10f, 20f);
      }

      UnityEngine.Object.Destroy(element.m_quality.GetComponent<Outline>());
      element.m_quality.text = $"<size=12>{stars}</size>";
      element.m_quality.color = starColor;

      // Parent size = 64x64, quality size = 20x20, top-right (0,0) -> (-4f,-10f)
      element.m_quality.rectTransform.anchoredPosition = new Vector2(-4f, -6f);

      // TODO: Spawned items might brake this, as they could have 99 stars. 
      // Possible fix, after x amount switch to: ★x[amount] = ★x99
    }
  }
  /*****************************/
  /*         ★★★☆☆         */
  /* Iron Mace                 */
  /* One-Handed                */
  /*                           */
  /* Blunt: 31 - 53            */
  /* Knockback: 90             */
  /* Backstab: 3x              */
  /*                           */
  /* Block: 11                 */
  /* Parry: 45                 */
  /* Parry bonus: 2x           */
  /*                           */
  /* Durability: 100/100       */
  /* Repair station level: 2   */
  /* _________________________ */
  /* Movement speed: -5%       */
  /* _________________________ */
  /* Set Bonus:                */
  /* Stealth +5%               */
  /*                           */
  /* Weight: 3                 */
  /* Crafted by: Mknoc         */
  /*****************************/
  static class CustomTooltip
  {
    private static readonly int starsSize = 22;

    // m_maxQuality > 1
    // - Add stars
    // m_dlc.Length > 0
    // - Add Item is DLC
    // m_teleportable = true
    // - Add teleportable

    private static void AddCrafted(StringBuilder sb, ItemDrop.ItemData item)
    {
      sb.AppendFormat("\n$item_crafter: <color=orange>{0}</color>", item.m_crafterName);
    }

    private static void AddDLC(StringBuilder sb, ItemDrop.ItemData item)
    {
      sb.Append("\n<color=aqua>$item_dlc</color>");
    }

    public static string ChangeTooltip(ItemDrop.ItemData item, int qualityLevel, bool crafting)
    {
      Player localPlayer = Player.m_localPlayer;
      StringBuilder stringBuilder = new StringBuilder(256);
      stringBuilder.Append(item.m_shared.m_description);
      stringBuilder.Append("\n");
      if (item.m_shared.m_maxQuality > 1)
      {
        //string stars = Helpers.Repeat("\u2605", qualityLevel); //new string('\u2605', itemData.m_quality);
                                                               //stars = string.Join("<size=8> </size>", stars.Split(' '));
        //string upgrades = new string('\u2606', item.m_shared.m_maxQuality - qualityLevel); // These go to Tooltip?
                                                                                           //Debug.Log($"{item.m_shared.m_name} {stars}{upgrades}");
        //stringBuilder.AppendFormat("\n<size={0}><color=yellow>{1}{2}</color></size>", starsSize, stars, upgrades);
      }
      stringBuilder.Append("\n");
      if (item.m_shared.m_dlc.Length > 0)
      {
        AddDLC(stringBuilder, item);
      }
      ItemDrop.ItemData.AddHandedTip(item, stringBuilder);
      /*
      if (item.m_crafterID != 0L)
      {
        stringBuilder.AppendFormat("\n$item_crafter: <color=orange>{0}</color>", item.m_crafterName);
      }
      */
      if (!item.m_shared.m_teleportable)
      {
        stringBuilder.Append("\n<color=orange>$item_noteleport</color>");
      }
      if (item.m_shared.m_value > 0)
      {
        stringBuilder.AppendFormat("\n$item_value: <color=orange>{0}  ({1})</color>", item.GetValue(), item.m_shared.m_value);
      }
      stringBuilder.AppendFormat("\n$item_weight: <color=orange>{0}</color>", item.GetWeight().ToString("0.0"));
      if (item.m_shared.m_maxQuality > 1)
      {
        stringBuilder.AppendFormat("\n$item_quality: <color=orange>{0}</color>", qualityLevel);
      }
      if (item.m_shared.m_useDurability)
      {
        if (crafting)
        {
          float maxDurability = item.GetMaxDurability(qualityLevel);
          stringBuilder.AppendFormat("\n$item_durability: <color=orange>{0}</color>", maxDurability);
        }
        else
        {
          float maxDurability2 = item.GetMaxDurability(qualityLevel);
          float durability = item.m_durability;
          stringBuilder.AppendFormat("\n$item_durability: <color=orange>{0}%</color> <color=yellow>({1}/{2})</color>", (item.GetDurabilityPercentage() * 100f).ToString("0"), durability.ToString("0"), maxDurability2.ToString("0"));
        }
        if (item.m_shared.m_canBeReparied)
        {
          Recipe recipe = ObjectDB.instance.GetRecipe(item);
          if (recipe != null)
          {
            int minStationLevel = recipe.m_minStationLevel;
            stringBuilder.AppendFormat("\n$item_repairlevel: <color=orange>{0}</color>", minStationLevel.ToString());
          }
        }
      }
      switch (item.m_shared.m_itemType)
      {
        case ItemDrop.ItemData.ItemType.Consumable:
          {
            if (item.m_shared.m_food > 0f)
            {
              stringBuilder.AppendFormat("\n$item_food_health: <color=orange>{0}</color>", item.m_shared.m_food);
              stringBuilder.AppendFormat("\n$item_food_stamina: <color=orange>{0}</color>", item.m_shared.m_foodStamina);
              stringBuilder.AppendFormat("\n$item_food_duration: <color=orange>{0}s</color>", item.m_shared.m_foodBurnTime);
              stringBuilder.AppendFormat("\n$item_food_regen: <color=orange>{0} hp/tick</color>", item.m_shared.m_foodRegen);
            }
            string statusEffectTooltip = item.GetStatusEffectTooltip();
            if (statusEffectTooltip.Length > 0)
            {
              stringBuilder.Append("\n\n");
              stringBuilder.Append(statusEffectTooltip);
            }
            break;
          }
        case ItemDrop.ItemData.ItemType.OneHandedWeapon:
        case ItemDrop.ItemData.ItemType.Bow:
        case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
        case ItemDrop.ItemData.ItemType.Torch:
          {
            stringBuilder.Append(item.GetDamage(qualityLevel).GetTooltipString(item.m_shared.m_skillType));
            stringBuilder.AppendFormat("\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", item.GetBaseBlockPower(qualityLevel), item.GetBlockPowerTooltip(qualityLevel).ToString("0"));
            if (item.m_shared.m_timedBlockBonus > 1f)
            {
              stringBuilder.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", item.GetDeflectionForce(qualityLevel));
              stringBuilder.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", item.m_shared.m_timedBlockBonus);
            }
            stringBuilder.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", item.m_shared.m_attackForce);
            stringBuilder.AppendFormat("\n$item_backstab: <color=orange>{0}x</color>", item.m_shared.m_backstabBonus);
            string projectileTooltip = item.GetProjectileTooltip(qualityLevel);
            if (projectileTooltip.Length > 0)
            {
              stringBuilder.Append("\n\n");
              stringBuilder.Append(projectileTooltip);
            }
            string statusEffectTooltip2 = item.GetStatusEffectTooltip();
            if (statusEffectTooltip2.Length > 0)
            {
              stringBuilder.Append("\n\n");
              stringBuilder.Append(statusEffectTooltip2);
            }
            break;
          }
        case ItemDrop.ItemData.ItemType.Shield:
          stringBuilder.AppendFormat("\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", item.GetBaseBlockPower(qualityLevel), item.GetBlockPowerTooltip(qualityLevel).ToString("0"));
          if (item.m_shared.m_timedBlockBonus > 1f)
          {
            stringBuilder.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", item.GetDeflectionForce(qualityLevel));
            stringBuilder.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", item.m_shared.m_timedBlockBonus);
          }
          break;
        case ItemDrop.ItemData.ItemType.Helmet:
        case ItemDrop.ItemData.ItemType.Chest:
        case ItemDrop.ItemData.ItemType.Legs:
        case ItemDrop.ItemData.ItemType.Shoulder:
          {
            stringBuilder.AppendFormat("\n$item_armor: <color=orange>{0}</color>", item.GetArmor(qualityLevel));
            string damageModifiersTooltipString = SE_Stats.GetDamageModifiersTooltipString(item.m_shared.m_damageModifiers);
            if (damageModifiersTooltipString.Length > 0)
            {
              stringBuilder.Append(damageModifiersTooltipString);
            }
            string statusEffectTooltip3 = item.GetStatusEffectTooltip();
            if (statusEffectTooltip3.Length > 0)
            {
              stringBuilder.Append("\n\n");
              stringBuilder.Append(statusEffectTooltip3);
            }
            break;
          }
        case ItemDrop.ItemData.ItemType.Ammo:
          stringBuilder.Append(item.GetDamage(qualityLevel).GetTooltipString(item.m_shared.m_skillType));
          stringBuilder.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", item.m_shared.m_attackForce);
          break;
      }
      if (item.m_shared.m_movementModifier != 0f && localPlayer != null)
      {
        float equipmentMovementModifier = localPlayer.GetEquipmentMovementModifier();
        stringBuilder.AppendFormat("\n$item_movement_modifier: <color=orange>{0}%</color> ($item_total:<color=yellow>{1}%</color>)", (item.m_shared.m_movementModifier * 100f).ToString("+0;-0"), (equipmentMovementModifier * 100f).ToString("+0;-0"));
      }
      string setStatusEffectTooltip = item.GetSetStatusEffectTooltip();
      if (setStatusEffectTooltip.Length > 0)
      {
        stringBuilder.AppendFormat("\n {0}", new string('\u2500', 30));
        stringBuilder.AppendFormat("\n$item_seteffect (<color=orange>{0}</color> $item_parts):\n<color=orange>{1}</color>", item.m_shared.m_setSize, setStatusEffectTooltip);
        stringBuilder.AppendFormat(" {0}\n", new string('\u2500', 30));
      }
      if (item.m_crafterID != 0L)
      {
        AddCrafted(stringBuilder, item);
        //stringBuilder.AppendFormat("\n$item_crafter: <color=orange>{0}</color>", item.m_crafterName);
      }
      return stringBuilder.ToString();
    }
  }
    // [HarmonyPatch(typeof(InventoryGrid), "CreateItemTooltip")]

    /* !! This is rendered everytime the Inventory GUI is open !! 
      * ITEM:
      * 
      * TOOLTIP:
      * tooltip.m_topic = item.m_shared.m_name
      * tooltip.m_text =
      * $item_hammer: $item_hammer_description
      * 
      * 
      * $item_crafter: <color=orange>Betterui</color>
      * $item_weight: <color=orange>2,0</color>
      * $item_quality: <color=orange>1</color>
      * $item_durability: <color=orange>99%</color> <color=yellow>(99/100)</color>
      * $item_repairlevel: <color=orange>1</color>
      * 
      * tooltip.transform sizeDelta = (64.0, 64.0) childCount = 9;
      * tooltip.transform is the item Element?
      * childs:
      * equiped: (32.0, -32.0)
      * queued: (32.0, -32.0)
      * icon: (32.0, -32.0)
      * amount: (32.0, -54.0)
      * durability: (32.0, -57.9)
      * binding: (10.0, -10.0)
      * quality: (60.0, -6.0)
      * selected: (32.0, -32.0)
      * noteleport: (64.0, 0.0)
      */

    // InventoryTooltip -> Bkg -> {Topic, Text}
    // Debug.Log($"{tooltip.m_topic}, {(tooltip.gameObject.transform as RectTransform).sizeDelta},{(tooltip.transform as RectTransform).sizeDelta}  {tooltip.m_tooltipPrefab}");
    /*
    if (tooltip.transform.childCount > 0)
    {
      for (int i = 0; i < tooltip.transform.childCount; i++)
      {
        //Transform child = tooltip.transform.GetChild(i);
        //Debug.Log($"{child.name}: {child.localPosition} {child.childCount} {child.GetType()}");
      }
    }
    */

    /*
    //HotkeyBar.ElementData invWeight = new HotkeyBar.ElementData();
    UnityEngine.Object.Destroy(__instance.transform.Find("ItemWeightHotBar"));
    GameObject invWeight = new GameObject("ItemWeightHotBar");
    invWeight.transform.SetParent(__instance.transform, false);
    invWeight.transform.localPosition = new Vector3(8f * __instance.m_elementSpace, 0f, 0f);
    invWeight.AddComponent<Text>().text = $"{player.GetInventory().GetTotalWeight()}/{player.GetMaxCarryWeight()}";
    invWeight.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf") as Font;
    */
    /*
    HotkeyBar.ElementData invWeight = new HotkeyBar.ElementData();
    invWeight.m_go = UnityEngine.Object.Instantiate<GameObject>(__instance.m_elementPrefab, __instance.transform);
    invWeight.m_go.transform.localPosition = new Vector3(8f * __instance.m_elementSpace, 0f, 0f);
    invWeight.m_amount = invWeight.m_go.transform.Find("amount").GetComponent<Text>();
    invWeight.m_amount.text = $"{player.GetInventory().GetTotalWeight()}/{player.GetMaxCarryWeight()}";
    __instance.m_elements.Add(invWeight);
    /*
    GameObject testObj = new GameObject("Testing");
    testObj.transform.SetParent(__instance.transform, false);
    testObj.AddComponent<Text>().text = $"{player.GetInventory().GetTotalWeight()}/{player.GetMaxCarryWeight()}";
    testObj.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf") as Font;
    */


    /*
    [HarmonyPatch(typeof(ItemDrop.ItemData), "GetTooltip", new Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(bool)})]
    public static class ChangeTooltipInfo
    {
      public static bool Prefix(ref string __result, ItemDrop.ItemData item, int qualityLevel, bool crafting)
      {
        Player localPlayer = Player.m_localPlayer;
        StringBuilder stringBuilder = new StringBuilder(256);
        stringBuilder.Append(item.m_shared.m_description);
        stringBuilder.Append("\n");
        if (item.m_shared.m_maxQuality > 1)
        {
          string stars = Helpers.Repeat("\u2605", qualityLevel); //new string('\u2605', itemData.m_quality);
          //stars = string.Join("<size=8> </size>", stars.Split(' '));
          string upgrades = new string('\u2606', item.m_shared.m_maxQuality - qualityLevel); // These go to Tooltip?
          //Debug.Log($"{item.m_shared.m_name} {stars}{upgrades}");
          stringBuilder.AppendFormat("\n<size=22><color=yellow>{0}{1}</color></size>", stars, upgrades);
        }
        stringBuilder.Append("\n");
        if (item.m_shared.m_dlc.Length > 0)
        {
          stringBuilder.Append("\n<color=aqua>$item_dlc</color>");
        }
        ItemDrop.ItemData.AddHandedTip(item, stringBuilder);
        if (item.m_crafterID != 0L)
        {
          stringBuilder.AppendFormat("\n$item_crafter: <color=orange>{0}</color>", item.m_crafterName);
        }
        if (!item.m_shared.m_teleportable)
        {
          stringBuilder.Append("\n<color=orange>$item_noteleport</color>");
        }
        if (item.m_shared.m_value > 0)
        {
          stringBuilder.AppendFormat("\n$item_value: <color=orange>{0}  ({1})</color>", item.GetValue(), item.m_shared.m_value);
        }
        stringBuilder.AppendFormat("\n$item_weight: <color=orange>{0}</color>", item.GetWeight().ToString("0.0"));
        if (item.m_shared.m_maxQuality > 1)
        {
          stringBuilder.AppendFormat("\n$item_quality: <color=orange>{0}</color>", qualityLevel);
        }
        if (item.m_shared.m_useDurability)
        {
          if (crafting)
          {
            float maxDurability = item.GetMaxDurability(qualityLevel);
            stringBuilder.AppendFormat("\n$item_durability: <color=orange>{0}</color>", maxDurability);
          }
          else
          {
            float maxDurability2 = item.GetMaxDurability(qualityLevel);
            float durability = item.m_durability;
            stringBuilder.AppendFormat("\n$item_durability: <color=orange>{0}%</color> <color=yellow>({1}/{2})</color>", (item.GetDurabilityPercentage() * 100f).ToString("0"), durability.ToString("0"), maxDurability2.ToString("0"));
          }
          if (item.m_shared.m_canBeReparied)
          {
            Recipe recipe = ObjectDB.instance.GetRecipe(item);
            if (recipe != null)
            {
              int minStationLevel = recipe.m_minStationLevel;
              stringBuilder.AppendFormat("\n$item_repairlevel: <color=orange>{0}</color>", minStationLevel.ToString());
            }
          }
        }
        switch (item.m_shared.m_itemType)
        {
          case ItemDrop.ItemData.ItemType.Consumable:
            {
              if (item.m_shared.m_food > 0f)
              {
                stringBuilder.AppendFormat("\n$item_food_health: <color=orange>{0}</color>", item.m_shared.m_food);
                stringBuilder.AppendFormat("\n$item_food_stamina: <color=orange>{0}</color>", item.m_shared.m_foodStamina);
                stringBuilder.AppendFormat("\n$item_food_duration: <color=orange>{0}s</color>", item.m_shared.m_foodBurnTime);
                stringBuilder.AppendFormat("\n$item_food_regen: <color=orange>{0} hp/tick</color>", item.m_shared.m_foodRegen);
              }
              string statusEffectTooltip = item.GetStatusEffectTooltip();
              if (statusEffectTooltip.Length > 0)
              {
                stringBuilder.Append("\n\n");
                stringBuilder.Append(statusEffectTooltip);
              }
              break;
            }
          case ItemDrop.ItemData.ItemType.OneHandedWeapon:
          case ItemDrop.ItemData.ItemType.Bow:
          case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
          case ItemDrop.ItemData.ItemType.Torch:
            {
              stringBuilder.Append(item.GetDamage(qualityLevel).GetTooltipString(item.m_shared.m_skillType));
              stringBuilder.AppendFormat("\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", item.GetBaseBlockPower(qualityLevel), item.GetBlockPowerTooltip(qualityLevel).ToString("0"));
              if (item.m_shared.m_timedBlockBonus > 1f)
              {
                stringBuilder.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", item.GetDeflectionForce(qualityLevel));
                stringBuilder.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", item.m_shared.m_timedBlockBonus);
              }
              stringBuilder.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", item.m_shared.m_attackForce);
              stringBuilder.AppendFormat("\n$item_backstab: <color=orange>{0}x</color>", item.m_shared.m_backstabBonus);
              string projectileTooltip = item.GetProjectileTooltip(qualityLevel);
              if (projectileTooltip.Length > 0)
              {
                stringBuilder.Append("\n\n");
                stringBuilder.Append(projectileTooltip);
              }
              string statusEffectTooltip2 = item.GetStatusEffectTooltip();
              if (statusEffectTooltip2.Length > 0)
              {
                stringBuilder.Append("\n\n");
                stringBuilder.Append(statusEffectTooltip2);
              }
              break;
            }
          case ItemDrop.ItemData.ItemType.Shield:
            stringBuilder.AppendFormat("\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", item.GetBaseBlockPower(qualityLevel), item.GetBlockPowerTooltip(qualityLevel).ToString("0"));
            if (item.m_shared.m_timedBlockBonus > 1f)
            {
              stringBuilder.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", item.GetDeflectionForce(qualityLevel));
              stringBuilder.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", item.m_shared.m_timedBlockBonus);
            }
            break;
          case ItemDrop.ItemData.ItemType.Helmet:
          case ItemDrop.ItemData.ItemType.Chest:
          case ItemDrop.ItemData.ItemType.Legs:
          case ItemDrop.ItemData.ItemType.Shoulder:
            {
              stringBuilder.AppendFormat("\n$item_armor: <color=orange>{0}</color>", item.GetArmor(qualityLevel));
              string damageModifiersTooltipString = SE_Stats.GetDamageModifiersTooltipString(item.m_shared.m_damageModifiers);
              if (damageModifiersTooltipString.Length > 0)
              {
                stringBuilder.Append(damageModifiersTooltipString);
              }
              string statusEffectTooltip3 = item.GetStatusEffectTooltip();
              if (statusEffectTooltip3.Length > 0)
              {
                stringBuilder.Append("\n\n");
                stringBuilder.Append(statusEffectTooltip3);
              }
              break;
            }
          case ItemDrop.ItemData.ItemType.Ammo:
            stringBuilder.Append(item.GetDamage(qualityLevel).GetTooltipString(item.m_shared.m_skillType));
            stringBuilder.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", item.m_shared.m_attackForce);
            break;
        }
        if (item.m_shared.m_movementModifier != 0f && localPlayer != null)
        {
          float equipmentMovementModifier = localPlayer.GetEquipmentMovementModifier();
          stringBuilder.AppendFormat("\n$item_movement_modifier: <color=orange>{0}%</color> ($item_total:<color=yellow>{1}%</color>)", (item.m_shared.m_movementModifier * 100f).ToString("+0;-0"), (equipmentMovementModifier * 100f).ToString("+0;-0"));
        }
        string setStatusEffectTooltip = item.GetSetStatusEffectTooltip();
        if (setStatusEffectTooltip.Length > 0)
        {
          stringBuilder.AppendFormat("\n\n$item_seteffect (<color=orange>{0}</color> $item_parts):<color=orange>{1}</color>", item.m_shared.m_setSize, setStatusEffectTooltip);
        }
        __result = stringBuilder.ToString();
        return false;
      }
    }
    */
}
