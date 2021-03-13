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

  public static class XPBar
  {
    private static readonly bool smoothFill = true;
    private static readonly bool smoothDrain = true;
    private static readonly float smoothSpeed = 0.7f;
    private static readonly float maxValue = 1f;
    private static Color barColor = Color.yellow;
    private static GuiBar _xp_bar = null;

    public static GuiBar Awake(Hud __instance)
    {
      Transform hudroot = Utils.FindChild(__instance.gameObject.transform, "hudroot");
      GuiBar xp_bar = UnityEngine.Object.Instantiate(Hud.instance.m_stealthBar, hudroot, true);
      xp_bar.m_barImage = Hud.instance.m_stealthBar.m_bar.GetComponent<Image>();
      xp_bar.m_smoothFill = smoothFill;
      xp_bar.m_smoothDrain = smoothDrain;
      xp_bar.m_smoothSpeed = smoothSpeed;
      xp_bar.Awake();

      xp_bar.name = "BU_XP_BAR";
      xp_bar.m_originalColor = barColor;
      xp_bar.ResetColor();
  
      xp_bar.SetMaxValue(maxValue);
      xp_bar.SetValue(0f);

      RectTransform xpRect = (xp_bar.transform as RectTransform);
      xpRect.anchorMin = Vector2.zero;
      xpRect.anchorMax = new Vector2(1f, 0f);
      xpRect.anchoredPosition = Vector2.zero;

      xpRect.offsetMin = Vector2.zero;
      xpRect.offsetMax = Vector2.zero;
      xpRect.sizeDelta = new Vector2(0f, 5f);

      xp_bar.m_bar.anchorMin = Vector2.zero;
      xp_bar.m_bar.anchorMax = new Vector2(1f, 0.5f);
      xp_bar.m_bar.offsetMin = Vector2.zero;
      xp_bar.m_bar.offsetMax = Vector2.zero;

      xp_bar.m_bar.sizeDelta = new Vector2(xpRect.rect.width, 5f);
      // Render the XP Bar 
      xp_bar.gameObject.SetActive(true);
      _xp_bar = xp_bar;

      return xp_bar;
    }
  
    public static void UpdatePosition()
    {
      if (_xp_bar == null) return;
      RectTransform xpRect = (_xp_bar.transform as RectTransform);
      if (xpRect == null) return;
      _xp_bar.m_bar.sizeDelta = xpRect.rect.size;
    }
  }

  public static class XP
  {
    private static readonly int fXPPerSkillRank = 1;
    private static Player RPG_Player;
    private static double xp_used;
    private static float xp_current;
    private static float xp_accumulator;

    public static int level;

    public static void Awake(Player player)
    {
      if (player == null)
      {
        Debug.LogWarning("Didn't get a player");
        return;
      }
      RPG_Player = player;

      UpdatePlayerXP();
      UpdatePlayerLevel();
      UpdateUsedXP();
    }

    public static void RaiseXP()
    {
      UpdatePlayerXP(); // TODO: Is it necessary to update all skills XP? As we could pass what skill got more xp and only calculate it?
      xp_accumulator = (float)(xp_current - xp_used);
      if (xp_accumulator >= GetNextLevelRequirement(level))
      {
        // Character level-up !!
        UpdatePlayerLevel();  // updates private level
        UpdateUsedXP();       // updates private xp_used

        RPG_Player.m_skillLevelupEffects.Create(RPG_Player.m_head.position, RPG_Player.m_head.rotation, RPG_Player.m_head, 1.5f);
        RPG_Player.Message(MessageHud.MessageType.Center, $"<size=60><color=white>LEVEL UP</color></size>\n<size=30>You are now Lv. {level} </size>");
      }
    }

    public static float GetLevelPercentage()
    {
      return (float)(xp_current - xp_used) / GetNextLevelRequirement(level);
    }

    private static void UpdatePlayerXP()
    {
      float player_xp = 0;
      if (RPG_Player == null) return;

      foreach (Skills.Skill skill in RPG_Player.m_skills.m_skillData.Values)
      {
        player_xp += CalulateGainedXP(skill.m_level);
        player_xp += (int)skill.m_level * skill.GetLevelPercentage();
      }
      xp_current = player_xp;
    }

    private static void UpdateUsedXP()
    {
      xp_used = 12.5 * (level * level) + 62.5 * level - 75;
    }

    private static void UpdatePlayerLevel()
    {
      level = Mathf.FloorToInt(-2.5f + Mathf.Sqrt(8 * xp_current + 1225) / 10);
    }

    private static float GetNextLevelRequirement(int lvl)
    {
      return (lvl + 3) * 25;
    }

    private static int CalulateGainedXP(float lvl)
    {
      return (int)lvl * ((int)lvl + 1) * fXPPerSkillRank / 2;
    }
  }
}
