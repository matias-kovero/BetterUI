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

  class CustomEnemyHud
  {
    private static readonly int bossFontSize = 16;
    private static readonly int defaultFontSize = 10;
    private static readonly string bossPrefix = "BU_hpTextBoss";
    private static readonly string defaultPrefix = "BU_hpText";
    public static readonly float maxDrawDistance = 50f;

    public static void AddLvlToName(EnemyHud __instance, Character c)
    {
      if (!Main.customEnemyHud.Value) return;

      EnemyHud.HudData hudData;
      if (__instance.m_huds.TryGetValue(c, out _)) return;

      GameObject original;
      if (c.IsPlayer()) original = __instance.m_baseHudPlayer;
      else if (c.IsBoss()) original = __instance.m_baseHudBoss;
      else original = __instance.m_baseHud;

      hudData = new EnemyHud.HudData
      {
        m_character = c,
        m_ai = c.GetComponent<BaseAI>(),
        m_gui = UnityEngine.Object.Instantiate(original, __instance.m_hudRoot.transform)
      };

      hudData.m_gui.SetActive(true);
      hudData.m_healthRoot = hudData.m_gui.transform.Find("Health").gameObject;
      hudData.m_healthFast = hudData.m_healthRoot.transform.Find("health_fast").GetComponent<GuiBar>();
      hudData.m_healthSlow = hudData.m_healthRoot.transform.Find("health_slow").GetComponent<GuiBar>();
      hudData.m_level2 = (hudData.m_gui.transform.Find("level_2") as RectTransform);
      hudData.m_level3 = (hudData.m_gui.transform.Find("level_3") as RectTransform);
      hudData.m_alerted = (hudData.m_gui.transform.Find("Alerted") as RectTransform);
      hudData.m_aware = (hudData.m_gui.transform.Find("Aware") as RectTransform);
      hudData.m_name = hudData.m_gui.transform.Find("Name").GetComponent<Text>();
      hudData.m_name.text = Localization.instance.Localize(c.GetHoverName());

      if (c.IsPlayer())
      {
        // Currently no edits to player
      }
      else if (c.IsBoss())
      {
        // Edits to Boss HP Bar
        Text hpText = UnityEngine.Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
        hpText.name = bossPrefix;
        hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 0.0f); // orig.y = 21f
        hpText.text = $"<size={bossFontSize}>{hudData.m_character.GetHealth():0} / {hudData.m_character.GetMaxHealth():0}</size>";
        hpText.color = Color.white;
        UnityEngine.Object.Destroy(hpText.GetComponent<Outline>());
      }
      else
      {
        hudData.m_name.fontSize = Main.enemyHudTextSize.Value;
        hudData.m_name.text = hudData.m_name.text.Insert(0, $"<size={Main.enemyHudTextSize.Value - 2}><color=white>Lv.{c.m_level} </color></size> ");
        Text hpText = UnityEngine.Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
        hpText.name = defaultPrefix;
        hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 6.0f); // orig.y = 21f
        hpText.text = $"<size={defaultFontSize}>{hudData.m_character.GetHealth():0}/{hudData.m_character.GetMaxHealth():0}</size>";
        hpText.color = Color.white;
        UnityEngine.Object.Destroy(hpText.GetComponent<Outline>());

        // Resize and position everything
        RectTransform hpRoot = (hudData.m_healthRoot.transform as RectTransform);
        Vector2 biggerBar = new Vector2(hpRoot.sizeDelta.x, hpRoot.sizeDelta.y * 3f);
        hpRoot.sizeDelta = biggerBar;
        hudData.m_alerted.gameObject.SetActive(false);
        hudData.m_aware.gameObject.SetActive(false);
        hudData.m_healthFast.m_bar.sizeDelta = new Vector2(hudData.m_healthFast.m_width, hpRoot.sizeDelta.y);
        hudData.m_healthSlow.m_bar.sizeDelta = new Vector2(hudData.m_healthSlow.m_width, hpRoot.sizeDelta.y);
      }
      __instance.m_huds.Add(c, hudData);
    }

    public static void UpdateHPText(EnemyHud __instance)
    {
      if (!Main.customEnemyHud.Value) return;

      Character character = null;
      foreach (KeyValuePair<Character, EnemyHud.HudData> keyValuePair in __instance.m_huds)
      {
        EnemyHud.HudData value = keyValuePair.Value;
        if (!value.m_character || !__instance.TestShow(value.m_character))
        {
          if (character == null)
          {
            character = value.m_character;
            UnityEngine.Object.Destroy(value.m_gui);
          }
        }
        else
        {
          if (value.m_character.IsPlayer())
          {

          }
          else if (value.m_character.IsBoss())
          {
            string hp = $"{value.m_character.GetHealth():0}";
            Utils.FindChild(value.m_name.transform.parent, bossPrefix).GetComponent<Text>().text = $"<size={bossFontSize}>{value.m_character.GetHealth():0} / {value.m_character.GetMaxHealth():0}</size>";
          }
          else
          {
            value.m_alerted.gameObject.SetActive(false);
            value.m_aware.gameObject.SetActive(false);

            bool aware = value.m_character.GetBaseAI().HaveTarget();
            bool alerted = value.m_character.GetBaseAI().IsAlerted();
            value.m_name.color = (aware || alerted) ? (alerted ? Color.red : Color.yellow) : Color.white;
            Utils.FindChild(value.m_name.transform.parent, defaultPrefix).GetComponent<Text>().text = $"<size={defaultFontSize}>{value.m_character.GetHealth():0}/{value.m_character.GetMaxHealth():0}</size>";
          }
        }
      }
      if (character != null)
      {
        __instance.m_huds.Remove(character);
      }
    }
  }
}
