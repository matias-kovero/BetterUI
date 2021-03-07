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
  
  static class InventoryArmorTooltip
  {
    // Should we just inherit from UITooltip and add our own stuff?
    public static UITooltip tooltip = null;
    public static Text m_armor = null;

    public static void Awake(InventoryGui ig)
    {
      Transform baContainer = ig.m_armor.transform.parent;

      // Save old values
      Transform background = Utils.FindChild(baContainer, "bkg");
      Transform icon = Utils.FindChild(baContainer, "armor_icon");
      Transform text = Utils.FindChild(baContainer, "ac_text");

      GameObject prefab = ig.m_containerGrid.m_elementPrefab;
      if (prefab == null) return;
      // Create an copy of Inventory Element, Important Components = Image, Tooltip
      GameObject InventoryElement = UnityEngine.Object.Instantiate<GameObject>(prefab, baContainer);

      // Deactivate stuff
      InventoryElement.transform.Find("equiped").GetComponent<Image>().enabled = false;
      InventoryElement.transform.Find("queued").GetComponent<Image>().enabled = false;
      InventoryElement.transform.Find("icon").GetComponent<Image>().enabled = false;
      InventoryElement.transform.Find("amount").GetComponent<Text>().enabled = false;
      InventoryElement.transform.Find("durability").gameObject.SetActive(false);
      InventoryElement.transform.Find("binding").GetComponent<Text>().enabled = false;
      InventoryElement.transform.Find("quality").GetComponent<Text>().enabled = false;
      InventoryElement.transform.Find("selected").gameObject.SetActive(false);
      InventoryElement.transform.Find("noteleport").GetComponent<Image>().enabled = false;

      // Delete components
      UnityEngine.Object.Destroy(InventoryElement.GetComponent<UIInputHandler>());
      UnityEngine.Object.Destroy(InventoryElement.GetComponent<Button>());
      // Update background
      InventoryElement.GetComponent<Image>().sprite = background.GetComponent<Image>().sprite;
      InventoryElement.GetComponent<Image>().color = background.GetComponent<Image>().color;
      InventoryElement.GetComponent<Image>().material = background.GetComponent<Image>().material;
      // Update position
      RectTransform oldBg = background as RectTransform;
      RectTransform newBg = InventoryElement.transform as RectTransform;

      newBg.anchoredPosition = oldBg.anchoredPosition;
      newBg.sizeDelta = oldBg.sizeDelta;

      icon.SetParent(InventoryElement.transform);
      text.SetParent(InventoryElement.transform);

      background.GetComponent<Image>().enabled = false;

      UITooltip bkgTooltip = InventoryElement.GetComponent<UITooltip>();
      bkgTooltip.m_topic = $"Lv.{XP.level} {Player.m_localPlayer.GetPlayerName()}";

      tooltip = bkgTooltip;
      //m_armor = text.GetComponent<Text>();
      m_armor = ig.m_armor;
    }
  
    public static void Update(Player player)
    {
      // Update UI info
      StringBuilder sb = new StringBuilder(256);

      WeaponStats(player, sb);
      BlockStats(player, sb);

      sb.AppendFormat("\n$item_armor: <color=orange>{0}</color>", Convert.ToInt32(player.GetBodyArmor()));

      sb.Append("\n" + new string('\u2500', 10) + "  Buffs  " + new string('\u2500', 10));
      if (player.m_equipmentMovementModifier != 0f)
      {
        string color = player.m_equipmentMovementModifier > 0 ? "green" : "red";
        sb.AppendFormat("\nMovement: <color={0}>{1}%</color>", color, player.m_equipmentMovementModifier * 100f);
      }

      /*
      List<StatusEffect> list = new List<StatusEffect>();
      player.GetSEMan().GetHUDStatusEffects(list);
      foreach (StatusEffect statusEffect in list)
      {
        sb.Append("<color=orange>" + Localization.instance.Localize(statusEffect.m_name) + "</color>\n");
        sb.Append(Localization.instance.Localize(statusEffect.GetTooltipString()));
        sb.Append("\n\n");
      }
      */
      sb.Append("\n");

      tooltip.m_text = sb.ToString();
    }
 
    private static void BlockStats(Player player, StringBuilder sb)
    {
      ItemDrop.ItemData cb = player.GetCurrentBlocker();
      if (cb != null)
      {
        float sf = player.GetSkillFactor(Skills.SkillType.Blocking);
        sb.AppendFormat("\n\n$item_blockpower: <color=orange>{0}</color>", Convert.ToInt32(cb.GetBlockPower(sf)));
        if (cb.m_shared.m_timedBlockBonus > 1f)
        {
          sb.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", cb.GetDeflectionForce(cb.m_quality));
          sb.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", cb.m_shared.m_timedBlockBonus);
        }
      }
    }
    
    private static void WeaponStats(Player player, StringBuilder sb)
    {
      ItemDrop.ItemData leftHand = GetHand(player, true); // Shield, Torch, 2H
      ItemDrop.ItemData rightHand = GetHand(player, false); // 1H
      ItemDrop.ItemData ammoItem = player.GetAmmoItem();

      // 1H at right hand, shield left

      // 2H at left hand
      if (leftHand != null)
      {
        if (leftHand.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield)
        {
          // Has shield, weapon in right hand.
        } else
        {
          HitData.DamageTypes hd = leftHand.GetDamage(leftHand.m_quality);

          if (leftHand.m_shared.m_skillType == Skills.SkillType.Bows && ammoItem != null)
          {
            hd.Add(ammoItem.GetDamage()); // Add current ammo to damage calculations.
          }
          sb.AppendFormat("{0}", hd.GetTooltipString(leftHand.m_shared.m_skillType));

          sb.AppendFormat("\n\n$item_knockback: <color=orange>{0}</color>", leftHand.m_shared.m_attackForce);
          sb.AppendFormat("\n$item_backstab: <color=orange>{0}x</color>", leftHand.m_shared.m_backstabBonus);
        }
      }

      if (rightHand != null)
      {
        HitData.DamageTypes hd = rightHand.GetDamage(rightHand.m_quality);

        // Show Item Damage
        sb.AppendFormat("{0}", hd.GetTooltipString(rightHand.m_shared.m_skillType));
        sb.AppendFormat("\n\n$item_knockback: <color=orange>{0}</color>", rightHand.m_shared.m_attackForce);
        sb.AppendFormat("\n$item_backstab: <color=orange>{0}x</color>", rightHand.m_shared.m_backstabBonus);
      }
    }

    private static ItemDrop.ItemData GetHand(Player player, bool left)
    {
      if (left)
      {
        return player.m_leftItem != null ? player.m_leftItem : player.m_hiddenLeftItem; 
      } else
      {
        return player.m_rightItem != null ? player.m_rightItem : player.m_hiddenRightItem;
      }

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
  /* Weight: 3.0               */
  /* Crafted by: BetterUI      */
  /*****************************/
  static class BetterTooltip
  {
    private static readonly int starsSize = 22;
    private static readonly char arrow = '\u2794';
    private static StringBuilder _sb;
    private static ItemDrop.ItemData _item;
    private static bool _crafting;
    private static int _quality;

    private static void Crafted()
    {
      _sb.AppendFormat("\n$item_crafter: <color=orange>{0}</color>", _item.m_crafterName);
    }

    private static void CustomDamageCalculations(int newLvl, int oldLvl)
    {
      HitData.DamageTypes oldItem = _item.GetDamage(oldLvl);
      HitData.DamageTypes newItem = _item.GetDamage(newLvl);
      if (newItem.m_damage > oldItem.m_damage)
      {
        _sb.AppendFormat("\n$inventory_damage: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_damage, arrow, newItem.m_damage);
      }
      if (newItem.m_blunt > oldItem.m_blunt)
      {
        _sb.AppendFormat("\n$inventory_blunt: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_blunt, arrow, newItem.m_blunt);
      }
      if (newItem.m_slash > oldItem.m_slash)
      {
        _sb.AppendFormat("\n$inventory_slash: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_slash, arrow, newItem.m_slash);
      }
      if (newItem.m_pierce > oldItem.m_pierce)
      {
        _sb.AppendFormat("\n$inventory_pierce: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_pierce, arrow, newItem.m_pierce);
      }
      if (newItem.m_fire > oldItem.m_fire)
      {
        _sb.AppendFormat("\n$inventory_fire: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_fire, arrow, newItem.m_fire);
      }
      if (newItem.m_frost > oldItem.m_frost)
      {
        _sb.AppendFormat("\n$inventory_frost: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_frost, arrow, newItem.m_frost);
      }
      if (newItem.m_lightning > oldItem.m_lightning)
      {
        _sb.AppendFormat("\n$inventory_lightning: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_lightning, arrow, newItem.m_lightning);
      }
      if (newItem.m_poison > oldItem.m_poison)
      {
        _sb.AppendFormat("\n$inventory_poison: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_poison, arrow, newItem.m_poison);
      }
      if (newItem.m_spirit > oldItem.m_spirit)
      {
        _sb.AppendFormat("\n$inventory_spirit: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldItem.m_spirit, arrow, newItem.m_spirit);
      }

    }

    private static void Description()
    {
      _sb.Append(_item.m_shared.m_description + "\n");
    }

    private static void DamageModifiers()
    {
      string damageModifiersTooltipString = SE_Stats.GetDamageModifiersTooltipString(_item.m_shared.m_damageModifiers);
      if (damageModifiersTooltipString.Length > 0)
      {
        _sb.Append(damageModifiersTooltipString);
      }
    }

    private static void DLC()
    {
      _sb.Append("\n<color=aqua>$item_dlc</color>");
    }

    private static void Durability(int qualityLevel, bool crafting)
    {
      if (crafting)
      {
        float maxDurability = _item.GetMaxDurability(qualityLevel);

        if (qualityLevel <= 1)
        {
          // Just creating item. Show base values.
          _sb.AppendFormat("\n$item_durability: <color=orange>{0}</color>", maxDurability);
        }
        else if (qualityLevel > _item.m_shared.m_maxQuality)
        {
          // No room to upgrade, game still shows upgraded values, so lets show the durability 1 lvl lower.
          float oldDurability = _item.GetMaxDurability(qualityLevel - 1);
          _sb.AppendFormat("\n$item_durability: <color=orange>{0}</color>", oldDurability);
        } else
        {
          float oldDurability = _item.GetMaxDurability(qualityLevel - 1);
          _sb.AppendFormat("\n$item_durability: <color=silver>{0}</color> {1} <color=orange>{2}</color>", oldDurability, arrow, maxDurability);
        }
      }
      else
      {
        float maxDurability2 = _item.GetMaxDurability(qualityLevel);
        float durability = _item.m_durability;
        //_sb.AppendFormat("\n$item_durability: <color=orange>{0}%</color> <color=yellow>({1}/{2})</color>", (_item.GetDurabilityPercentage() * 100f).ToString("0"), durability.ToString("0"), maxDurability2.ToString("0"));
        _sb.AppendFormat("\n$item_durability: {0} / {1}", durability.ToString("0"), maxDurability2.ToString("0"));
      }
    }

    private static void ItemType(int qualityLevel)
    {
      switch (_item.m_shared.m_itemType)
      {
        case ItemDrop.ItemData.ItemType.Consumable:
          {
            if (_item.m_shared.m_food > 0f)
            {
              _sb.AppendFormat("\n$item_food_health: <color=orange>{0}</color>", _item.m_shared.m_food);
              _sb.AppendFormat("\n$item_food_stamina: <color=orange>{0}</color>", _item.m_shared.m_foodStamina);
              _sb.AppendFormat("\n$item_food_duration: <color=orange>{0}s</color>", _item.m_shared.m_foodBurnTime);
              _sb.AppendFormat("\n$item_food_regen: <color=orange>{0} hp/tick</color>", _item.m_shared.m_foodRegen);
            }
            string statusEffectTooltip = _item.GetStatusEffectTooltip();
            if (statusEffectTooltip.Length > 0)
            {
              _sb.Append("\n\n");
              _sb.Append(statusEffectTooltip);
            }
            break;
          }
        case ItemDrop.ItemData.ItemType.OneHandedWeapon:
        case ItemDrop.ItemData.ItemType.Bow:
        case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
        case ItemDrop.ItemData.ItemType.Torch:
          {
            _sb.Append(_item.GetDamage(qualityLevel).GetTooltipString(_item.m_shared.m_skillType));
            _sb.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", _item.m_shared.m_attackForce);
            _sb.AppendFormat("\n$item_backstab: <color=orange>{0}x</color>", _item.m_shared.m_backstabBonus);

            _sb.AppendFormat("\n\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", _item.GetBaseBlockPower(qualityLevel), _item.GetBlockPowerTooltip(qualityLevel).ToString("0"));
            if (_item.m_shared.m_timedBlockBonus > 1f)
            {
              _sb.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", _item.GetDeflectionForce(qualityLevel));
              _sb.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", _item.m_shared.m_timedBlockBonus);
            }
            string projectileTooltip = _item.GetProjectileTooltip(qualityLevel);
            if (projectileTooltip.Length > 0)
            {
              _sb.Append("\n\n");
              _sb.Append(projectileTooltip);
            }
            string statusEffectTooltip2 = _item.GetStatusEffectTooltip();
            if (statusEffectTooltip2.Length > 0)
            {
              _sb.Append("\n\n");
              _sb.Append(statusEffectTooltip2);
            }
            _sb.Append("\n");
            break;
          }
        case ItemDrop.ItemData.ItemType.Shield:
          _sb.AppendFormat("\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", _item.GetBaseBlockPower(qualityLevel), _item.GetBlockPowerTooltip(qualityLevel).ToString("0"));
          if (_item.m_shared.m_timedBlockBonus > 1f)
          {
            _sb.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", _item.GetDeflectionForce(qualityLevel));
            _sb.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", _item.m_shared.m_timedBlockBonus);
            _sb.Append("\n");
          }
          break;
        case ItemDrop.ItemData.ItemType.Helmet:
        case ItemDrop.ItemData.ItemType.Chest:
        case ItemDrop.ItemData.ItemType.Legs:
        case ItemDrop.ItemData.ItemType.Shoulder:
          {
            _sb.AppendFormat("\n$item_armor: <color=orange>{0}</color>", _item.GetArmor(qualityLevel));
            /*
            string damageModifiersTooltipString = SE_Stats.GetDamageModifiersTooltipString(_item.m_shared.m_damageModifiers);
            if (damageModifiersTooltipString.Length > 0)
            {
              _sb.Append(damageModifiersTooltipString);
            }
            */
            string statusEffectTooltip3 = _item.GetStatusEffectTooltip();
            if (statusEffectTooltip3.Length > 0)
            {
              _sb.Append("\n\n");
              _sb.Append(statusEffectTooltip3);
            }
            break;
          }
        case ItemDrop.ItemData.ItemType.Ammo:
          _sb.Append(_item.GetDamage(qualityLevel).GetTooltipString(_item.m_shared.m_skillType));
          _sb.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", _item.m_shared.m_attackForce);
          break;
      }
    }

    private static void Movement(Player localPlayer)
    {
      //float equipmentMovementModifier = localPlayer.GetEquipmentMovementModifier();
      string color = localPlayer.m_equipmentMovementModifier > 0 ? "green" : "red";
      _sb.AppendFormat("\n$item_movement_modifier: <color={0}>{1}%</color>", color, (_item.m_shared.m_movementModifier * 100f).ToString("+0;-0"));
    }

    private static void Quality(int qualityLevel)
    {
      _sb.AppendFormat("\n$item_quality: <color=orange>{0}</color>", qualityLevel);
    }

    private static void RepairStation()
    {
      Recipe recipe = ObjectDB.instance.GetRecipe(_item);
      if (recipe != null)
      {
        int minStationLevel = recipe.m_minStationLevel;
        _sb.AppendFormat("\n$item_repairlevel: <color=orange>{0}</color>", minStationLevel.ToString());
      }
    }

    private static void Stars(int qualityLevel)
    {
      // Naive, Please FIX
      string stars = Helpers.Repeat("\u2605", qualityLevel);
      //string upgrades = new string('\u2606', item.m_shared.m_maxQuality - qualityLevel); // These go to Tooltip?
      _sb.AppendFormat("\n<size={0}><color=yellow>{1}</color></size>", starsSize, stars); // {2} upgrades
    }

    private static void StatusEffect(string effectText)
    {
      /*
      _sb.AppendFormat("\n {0}", new string('\u2500', 25));
      _sb.AppendFormat("\n$item_seteffect (<color=orange>{0}</color> $item_parts):\n<color=orange>{1}</color>", _item.m_shared.m_setSize, effectText);
      _sb.AppendFormat(" {0}\n", new string('\u2500', 25));
      */
      string setSize = $"$item_seteffect ({_item.m_shared.m_setSize})";
      _sb.AppendFormat("\n\n<color=silver>{0}</color>", setSize);
      _sb.AppendFormat("\n<color=orange>{0}</color>", effectText);
    }

    private static void Teleport()
    {
      _sb.Append("\n<color=red>$item_noteleport</color>");
    }

    private static void UpgradeStats()
    {
      int newQuality = _quality;
      int oldQuality = _quality - 1;

      switch (_item.m_shared.m_itemType)
      {
        case ItemDrop.ItemData.ItemType.OneHandedWeapon:
        case ItemDrop.ItemData.ItemType.Bow:
        case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
        case ItemDrop.ItemData.ItemType.Torch:
          {
            if (_item.GetDamage(newQuality).GetTotalDamage() > _item.GetDamage(oldQuality).GetTotalDamage())
            {
              CustomDamageCalculations(newQuality, oldQuality);
            } else
            {
              _sb.Append(_item.GetDamage(newQuality).GetTooltipString(_item.m_shared.m_skillType));
            }
            _sb.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", _item.m_shared.m_attackForce);
            _sb.AppendFormat("\n$item_backstab: <color=orange>{0}x</color>", _item.m_shared.m_backstabBonus);

            if (_item.GetBaseBlockPower(newQuality) > _item.GetBaseBlockPower(oldQuality))
            {
              _sb.AppendFormat("\n\n$item_blockpower: <color=silver>{0}</color> {1} <color=orange>{2}</color>", _item.GetBaseBlockPower(oldQuality), arrow, _item.GetBaseBlockPower(newQuality));
            }
            else
            {
              _sb.AppendFormat("\n\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", _item.GetBaseBlockPower(newQuality), _item.GetBlockPowerTooltip(newQuality).ToString("0"));
            }
            if (_item.m_shared.m_timedBlockBonus > 1f)
            {
              if (_item.GetDeflectionForce(newQuality) > _item.GetDeflectionForce(oldQuality))
              {
                _sb.AppendFormat("\n$item_deflection: <color=silver>{0}</color> {1} <color=orange>{2}</color>", _item.GetDeflectionForce(oldQuality), arrow, _item.GetDeflectionForce(newQuality));
              }
              else
              {
                _sb.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", _item.GetDeflectionForce(newQuality));
              }
              _sb.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", _item.m_shared.m_timedBlockBonus);
            }
            string projectileTooltip = _item.GetProjectileTooltip(newQuality);
            if (projectileTooltip.Length > 0)
            {
              _sb.Append("\n\n");
              _sb.Append(projectileTooltip);
            }
            string statusEffectTooltip2 = _item.GetStatusEffectTooltip();
            if (statusEffectTooltip2.Length > 0)
            {
              _sb.Append("\n\n");
              _sb.Append(statusEffectTooltip2);
            }
            _sb.Append("\n");
            break;
          }
        case ItemDrop.ItemData.ItemType.Shield:
          if (_item.GetBaseBlockPower(newQuality) > _item.GetBaseBlockPower(oldQuality))
          {
            _sb.AppendFormat("\n$item_blockpower: <color=silver>{0}</color> {1} <color=orange>{2}</color>", _item.GetBaseBlockPower(oldQuality), arrow, _item.GetBaseBlockPower(newQuality));
          } else
          {
            _sb.AppendFormat("\n$item_blockpower: <color=orange>{0}</color> <color=yellow>({1})</color>", _item.GetBaseBlockPower(newQuality), _item.GetBlockPowerTooltip(newQuality).ToString("0"));
          }
          if (_item.m_shared.m_timedBlockBonus > 1f)
          {
            if (_item.GetDeflectionForce(newQuality) > _item.GetDeflectionForce(oldQuality))
            {
              _sb.AppendFormat("\n$item_deflection: <color=silver>{0}</color> {1} <color=orange>{2}</color>", _item.GetDeflectionForce(oldQuality), arrow, _item.GetDeflectionForce(newQuality));
            } else
            {
              _sb.AppendFormat("\n$item_deflection: <color=orange>{0}</color>", _item.GetDeflectionForce(newQuality));
            }
            _sb.AppendFormat("\n$item_parrybonus: <color=orange>{0}x</color>", _item.m_shared.m_timedBlockBonus);
            _sb.Append("\n");
          }
          break;
        case ItemDrop.ItemData.ItemType.Helmet:
        case ItemDrop.ItemData.ItemType.Chest:
        case ItemDrop.ItemData.ItemType.Legs:
        case ItemDrop.ItemData.ItemType.Shoulder:
          {
            if(_item.GetArmor(newQuality) > _item.GetArmor(oldQuality))
            {
              _sb.AppendFormat("\n$item_armor: <color=silver>{0}</color> {1} <color=orange>{2}</color>", _item.GetArmor(oldQuality), arrow, _item.GetArmor(newQuality));
            } else
            {
              _sb.AppendFormat("\n$item_armor: <color=orange>{0}</color>", _item.GetArmor(newQuality));
            }
            /*
            string damageModifiersTooltipString = SE_Stats.GetDamageModifiersTooltipString(_item.m_shared.m_damageModifiers);
            if (damageModifiersTooltipString.Length > 0)
            {
              _sb.Append(damageModifiersTooltipString);
            }
            */
            string statusEffectTooltip3 = _item.GetStatusEffectTooltip();
            if (statusEffectTooltip3.Length > 0)
            {
              _sb.Append("\n\n");
              _sb.Append(statusEffectTooltip3);
            }
            break;
          }
        case ItemDrop.ItemData.ItemType.Ammo:
          _sb.Append(_item.GetDamage(newQuality).GetTooltipString(_item.m_shared.m_skillType));
          _sb.AppendFormat("\n$item_knockback: <color=orange>{0}</color>", _item.m_shared.m_attackForce);
          break;

      }
    }

    private static void Value()
    {
      _sb.AppendFormat("\n$item_value: <color=orange>{0}  ({1})</color>", _item.GetValue(), _item.m_shared.m_value);
    }

    private static void Weight()
    {
      _sb.AppendFormat("\n\n$item_weight: <color=orange>{0}</color>", _item.GetWeight().ToString("F1"));
    }

    private static void WieldType()
    {
      // Appends: \n$item_onehanded or \n$item_twohanded
      StringBuilder temp = new StringBuilder();
      ItemDrop.ItemData.AddHandedTip(_item, temp);

      _sb.AppendFormat("<color=silver>{0}</color>", temp);
    }

    public static string Create(ItemDrop.ItemData item, int qualityLevel, bool crafting)
    {
      Player localPlayer = Player.m_localPlayer;

      _sb = new StringBuilder(256);
      _item = item;
      _quality = qualityLevel;
      _crafting = crafting;

      Description();

      /* Scenarios:
       * Hover Tooltip:
       *  - Normal
       * 
       * Crafting Tooltip:
       *  - Creating new item: qualityLevel <= 1
       *    - Normal
       *  - Maxed item: qualityLevel >= m_shared.m_maxQuality
       *    - Show stats qualityLevel - 1.
       *  - Upgrading:
       *    - Calculate diffrences.
       */

      // Item has potential on upgrading, as is not max quality
      if (crafting && _item.m_shared.m_maxQuality > 1)
      {
        if (qualityLevel <= 1) BasicTooltip(localPlayer);
        else if (qualityLevel > _item.m_shared.m_maxQuality) CraftingTooltip(localPlayer, true);// Item is maxed.
        else CraftingTooltip(localPlayer);
      }
      else
      {
        BasicTooltip(localPlayer);
      }
      Weight();

      if (_item.m_crafterID != 0L) Crafted();

      return _sb.ToString();
    }

    private static void BasicTooltip(Player localPlayer, bool isMax = false)
    {
      if (_item.m_shared.m_dlc.Length > 0) DLC();

      WieldType();

      if (!_item.m_shared.m_teleportable) Teleport();
      if (_item.m_shared.m_value > 0) Value();

      ItemType(_quality);

      if (_item.m_shared.m_useDurability)
      {
        if (isMax) Durability(_quality + 1, _crafting); // Retarded fix, as Durability has its own logic.
        else Durability(_quality, _crafting);

        if (_item.m_shared.m_canBeReparied) RepairStation();
        _sb.Append("\n");
      }

      if (_item.m_shared.m_movementModifier != 0f && localPlayer != null) Movement(localPlayer);

      DamageModifiers();

      string setStatusEffectTooltip = _item.GetSetStatusEffectTooltip();
      if (setStatusEffectTooltip.Length > 0) StatusEffect(setStatusEffectTooltip);
    }

    private static void CraftingTooltip(Player localPlayer, bool isMax = false)
    {
      // Item is not max lvl, check the change of stats if user would update.
      if (isMax)
      {
        _quality -= 1;
        BasicTooltip(localPlayer, true);
      }
      else
      {
        if (_item.m_shared.m_dlc.Length > 0) DLC();

        WieldType();

        if (!_item.m_shared.m_teleportable) Teleport();
        if (_item.m_shared.m_value > 0) Value();

        // Your fantastic logic to parse stats.
        UpgradeStats();

        if (_item.m_shared.m_useDurability)
        {
          Durability(_quality, _crafting);
          if (_item.m_shared.m_canBeReparied) RepairStation();
          _sb.Append("\n");
        }

        if (_item.m_shared.m_movementModifier != 0f && localPlayer != null) Movement(localPlayer);

        DamageModifiers();

        string setStatusEffectTooltip = _item.GetSetStatusEffectTooltip();
        if (setStatusEffectTooltip.Length > 0) StatusEffect(setStatusEffectTooltip);
      }

      // Ex. 
      // Block: 55 -> 60
      // Durability: 1400 -> 1600

    }
  }
}
