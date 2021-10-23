using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  public static class BetterEnemyHud
  {
    private static readonly int _bossHPFontSize = 16;
    private static readonly int _hpFontSize = 10;
    private static readonly string _bossHPPrefix = "BU_bossHPText";
    private static readonly string _hpPrefix = "BU_hpText";

    public static readonly float maxDrawDistance = 3f;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyHud), "Awake")]
    private static void PatchDefaults(ref EnemyHud __instance)
    {
      float limiter = Mathf.Min(Mathf.Abs(Main.maxShowDistance.Value), maxDrawDistance);
      __instance.m_maxShowDistance *= limiter;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyHud), "ShowHud")]
    private static void PatchName(ref EnemyHud __instance, ref Character c, bool isMount)
    {
      if (!Main.customEnemyHud.Value) return;

        EnemyHud.HudData hudData;
        if (__instance.m_huds.TryGetValue(c, out hudData))
        {
            return;
        }
        GameObject original;
        if (isMount)
        {
            original = __instance.m_baseHudMount;
        }
        else if (c.IsPlayer())
        {
            original = __instance.m_baseHudPlayer;
        }
        else if (c.IsBoss())
        {
            original = __instance.m_baseHudBoss;
        }
        else
        {
            original = __instance.m_baseHud;
        }
        hudData = new EnemyHud.HudData();
        hudData.m_character = c;
        hudData.m_ai = c.GetComponent<BaseAI>();
        hudData.m_gui = UnityEngine.Object.Instantiate<GameObject>(original, __instance.m_hudRoot.transform);
        hudData.m_gui.SetActive(true);
        hudData.m_healthFast = hudData.m_gui.transform.Find("Health/health_fast").GetComponent<GuiBar>();
        hudData.m_healthSlow = hudData.m_gui.transform.Find("Health/health_slow").GetComponent<GuiBar>();
        if (isMount)
        {
            hudData.m_stamina = hudData.m_gui.transform.Find("Stamina/stamina_fast").GetComponent<GuiBar>();
            hudData.m_staminaText = hudData.m_gui.transform.Find("Stamina/StaminaText").GetComponent<Text>();
            hudData.m_healthText = hudData.m_gui.transform.Find("Health/HealthText").GetComponent<Text>();
        }
        hudData.m_level2 = (hudData.m_gui.transform.Find("level_2") as RectTransform);
        hudData.m_level3 = (hudData.m_gui.transform.Find("level_3") as RectTransform);
        hudData.m_alerted = (hudData.m_gui.transform.Find("Alerted") as RectTransform);
        hudData.m_aware = (hudData.m_gui.transform.Find("Aware") as RectTransform);
        hudData.m_name = hudData.m_gui.transform.Find("Name").GetComponent<Text>();
        hudData.m_name.text = Localization.instance.Localize(c.GetHoverName());
        hudData.m_isMount = isMount;

        if (c.IsPlayer())
      {
        // Currently no edits to player
      }
      else if (c.IsBoss())
      {
        if (!Main.hideEnemyHPText.Value)
        {
          // Edits to Boss HP Bar
          Text hpText = Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
          hpText.name = _bossHPPrefix;
          hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 0.0f); // orig.y = 21f
          hpText.text = $"<size={_bossHPFontSize}>{hudData.m_character.GetHealth():0} / {hudData.m_character.GetMaxHealth():0}</size>";
          hpText.color = Color.white;
          Object.Destroy(hpText.GetComponent<Outline>());
        }
      }
      else
      {
        hudData.m_name.fontSize = Main.enemyHudTextSize.Value;
        if (Main.enemyLvlStyle.Value != 0)
        {
          hudData.m_name.text = hudData.m_name.text.Insert(0, $"<size={Main.enemyHudTextSize.Value - 2}><color=white>Lv.{c.m_level} </color></size> ");
        }
        if (!Main.hideEnemyHPText.Value)
        {
          Text hpText = Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
          hpText.name = _hpPrefix;
          hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 7.0f); // orig.y = 21f
          hpText.text = $"<size={_hpFontSize}>{hudData.m_character.GetHealth():0}/{hudData.m_character.GetMaxHealth():0}</size>";
          hpText.color = Color.white;
          Object.Destroy(hpText.GetComponent<Outline>());
        }

        // Resize and position everything
        RectTransform hpRoot = (hudData.m_gui.transform.Find("Health") as RectTransform);
        Vector2 biggerBar = new Vector2(hpRoot.sizeDelta.x, hpRoot.sizeDelta.y * 3f);
        hpRoot.sizeDelta = biggerBar;
        hudData.m_alerted.gameObject.SetActive(false);
        hudData.m_aware.gameObject.SetActive(false);
        hudData.m_healthFast.m_bar.sizeDelta = new Vector2(hudData.m_healthFast.m_width, hpRoot.sizeDelta.y);
        hudData.m_healthSlow.m_bar.sizeDelta = new Vector2(hudData.m_healthSlow.m_width, hpRoot.sizeDelta.y);

        if (Main.enemyLvlStyle.Value == 1)
        {
          hudData.m_level2.gameObject.SetActive(false);
          hudData.m_level3.gameObject.SetActive(false);
        }
      }
      __instance.m_huds.Add(c, hudData);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyHud), "UpdateHuds")]
    private static void UpdateHP(ref EnemyHud __instance, Player player, Sadle sadle, float dt)
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
            Object.Destroy(value.m_gui);
          }
        }
        else
        {
          if (value.m_character.IsPlayer())
          {
            // Currently no edits to player
          }
          else if (value.m_character.IsBoss())
          {
            if (!Main.hideEnemyHPText.Value)
            {
              Utils.FindChild(value.m_name.transform.parent, _bossHPPrefix).GetComponent<Text>().text = $"<size={_bossHPFontSize}>{Mathf.CeilToInt(value.m_character.GetHealth())} / {value.m_character.GetMaxHealth():0}</size>";
            }
          }
          else
          {
            value.m_alerted.gameObject.SetActive(false);
            value.m_aware.gameObject.SetActive(false);

            bool aware = value.m_character.GetBaseAI().HaveTarget();
            bool alerted = value.m_character.GetBaseAI().IsAlerted();
            value.m_name.color = (aware || alerted) ? (alerted ? Color.red : Color.yellow) : Color.white;

            if (!Main.hideEnemyHPText.Value)
            {
              Utils.FindChild(value.m_name.transform.parent, _hpPrefix).GetComponent<Text>().text = $"<size={_hpFontSize}>{Mathf.CeilToInt(value.m_character.GetHealth())}/{value.m_character.GetMaxHealth():0}</size>";
            }

            if (Main.enemyLvlStyle.Value == 1)
            {
              value.m_level2.gameObject.SetActive(false);
              value.m_level3.gameObject.SetActive(false);
            }
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
