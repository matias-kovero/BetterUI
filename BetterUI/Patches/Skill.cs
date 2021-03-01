using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Unity;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
  class XPNotification
  {
    private static readonly int fontSize = 14;

    public static void Show(Skills.Skill skill, float factor)
    {
      float acc = (float)Math.Round(skill.m_accumulator * 100f) / 100f;
      float max = (float)Math.Round(skill.GetNextLevelRequirement() * 100f) / 100f;
      string str = $"XP for {skill.m_info.m_skill} (+{skill.m_info.m_increseStep * factor})\n[{acc}/{max}] ({skill.GetLevelPercentage() * 100f:0.##}%)";
      MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"<size={fontSize}>{str}</size>");
    }
  }

  class SkillUI
  {
    private static readonly float paddingFix = 0.15f;

    public static void UpdateDialog(SkillsDialog dialog, Player player)
    {
      foreach (GameObject obj in dialog.m_elements)
      {
        UnityEngine.Object.Destroy(obj);
      }
      dialog.m_elements.Clear();
      List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
      for (int i = 0; i < skillList.Count; i++)
      {
        Skills.Skill skill = skillList[i];
        float acc = (float)Math.Round(skill.m_accumulator * 100f) / 100f;
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(dialog.m_elementPrefab, Vector3.zero, Quaternion.identity, dialog.m_listRoot);

        gameObject.SetActive(true);
        (gameObject.transform as RectTransform).anchoredPosition = new Vector2(0f, (float)(-(float)i - paddingFix) * dialog.m_spacing);
        //UITooltip t = gameObject.GetComponentInChildren<UITooltip>();
        gameObject.GetComponentInChildren<UITooltip>().m_text = skill.m_info.m_description;//.m_text = skill.m_info.m_description;

        Utils.FindChild(gameObject.transform, "icon").GetComponent<Image>().sprite = skill.m_info.m_icon;
        Utils.FindChild(gameObject.transform, "name").GetComponent<Text>().text = Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower() + $"\n<size={Main.skillUITextSize.Value - 2}>Lvl: {(int)skill.m_level}</size>");
        Utils.FindChild(gameObject.transform, "name").GetComponent<Text>().fontSize = Main.skillUITextSize.Value;
        Utils.FindChild(gameObject.transform, "leveltext").GetComponent<Text>().text = $"<size=10>{acc} ({skill.GetLevelPercentage() * 100f:0.##}%)</size>";
        Utils.FindChild(gameObject.transform, "levelbar").GetComponent<GuiBar>().SetValue(skill.GetLevelPercentage());

        // Alter existing xpBar size to fill currentlevel area as well.
        RectTransform xpBar = (Utils.FindChild(gameObject.transform, "levelbar").GetComponent<GuiBar>().m_bar.parent as RectTransform);
        xpBar.sizeDelta = new Vector2(xpBar.sizeDelta.x, xpBar.sizeDelta.y + 4f);
        xpBar.anchoredPosition = new Vector2(-4f, 0f);
        RectTransform txt = Utils.FindChild(gameObject.transform, "leveltext").GetComponent<Text>().rectTransform;
        txt.anchoredPosition = new Vector2(txt.anchoredPosition.x, txt.anchoredPosition.y + 2f);
        // Remove currentlevel bar
        UnityEngine.Object.Destroy(Utils.FindChild(gameObject.transform, "currentlevel").GetComponent<GuiBar>().gameObject);

        dialog.m_elements.Add(gameObject);
      }
      float size = Mathf.Max(dialog.m_baseListSize, ((float)skillList.Count + paddingFix) * dialog.m_spacing);
      dialog.m_listRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

      // Devs added this, but forgot to render it...
      /*
      __instance.m_totalSkillText.text = string.Concat(new string[]
      {
          "<color=orange>",
          player.GetSkills().GetTotalSkill().ToString("0"),
          "</color><color=white> / </color><color=orange>",
          player.GetSkills().GetTotalSkillCap().ToString("0"),
          "</color>"
      });
      */
    }
  }
  /*
  class Skill
  {
    [HarmonyPatch(typeof(Skills), "RaiseSkill")]
    public static class SkillInfo
    {

      private static void Postfix(ref Skills __instance, ref Skills.SkillType skillType, ref float factor)
      {
        if (Main.showXPNotifications.Value)
        {
          Skills.Skill skill = __instance.GetSkill(skillType);
          float acc = (float)Math.Round(skill.m_accumulator * 100f) / 100f;
          float max = (float)Math.Round(skill.GetNextLevelRequirement() * 100f) / 100f;
          string str = $"XP for {skill.m_info.m_skill} (+{skill.m_info.m_increseStep * factor})\n[{acc}/{max}] ({skill.GetLevelPercentage() * 100f:0.##}%)";
          MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"<size=14>{str}</size>");
        }
                
      }
    }

    [HarmonyPatch(typeof(SkillsDialog), "Setup")]
    public static class SkillGUI
    {
      private static float paddingFix = 0.15f;

      private static void Postfix(ref SkillsDialog __instance, ref Player player) 
      {
        __instance.gameObject.SetActive(true);
        if (Main.showCharacterXP.Value)
        {
          Utils.FindChild(__instance.transform, "topic").GetComponent<Text>().text = $"LV. {XP.level:0}";
        }
        if (!Main.customSkillUI.Value) return;

        foreach (GameObject obj in __instance.m_elements)
        {
          UnityEngine.Object.Destroy(obj);
        }
        __instance.m_elements.Clear();
        List<Skills.Skill> skillList = player.GetSkills().GetSkillList();
        for (int i = 0; i < skillList.Count; i++)
        {
          Skills.Skill skill = skillList[i];
          float acc = (float)Math.Round(skill.m_accumulator * 100f) / 100f;
          GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.m_elementPrefab, Vector3.zero, Quaternion.identity, __instance.m_listRoot);

          gameObject.SetActive(true);
          (gameObject.transform as RectTransform).anchoredPosition = new Vector2(0f, (float)(-(float)i - paddingFix) * __instance.m_spacing);
          //UITooltip t = gameObject.GetComponentInChildren<UITooltip>();
          gameObject.GetComponentInChildren<UITooltip>().m_text = skill.m_info.m_description;//.m_text = skill.m_info.m_description;

          Utils.FindChild(gameObject.transform, "icon").GetComponent<Image>().sprite = skill.m_info.m_icon;
          Utils.FindChild(gameObject.transform, "name").GetComponent<Text>().text = Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower() + $"\n<size={Main.skillUITextSize.Value - 2}>Lvl: {(int)skill.m_level}</size>");
          Utils.FindChild(gameObject.transform, "name").GetComponent<Text>().fontSize = Main.skillUITextSize.Value;
          Utils.FindChild(gameObject.transform, "leveltext").GetComponent<Text>().text = $"<size=10>{acc} ({skill.GetLevelPercentage() * 100f:0.##}%)</size>";
          Utils.FindChild(gameObject.transform, "levelbar").GetComponent<GuiBar>().SetValue(skill.GetLevelPercentage());

          // Alter existing xpBar size to fill currentlevel area aswell.
          RectTransform xpBar = (Utils.FindChild(gameObject.transform, "levelbar").GetComponent<GuiBar>().m_bar.parent as RectTransform);
          xpBar.sizeDelta = new Vector2(xpBar.sizeDelta.x, xpBar.sizeDelta.y + 4f);
          xpBar.anchoredPosition = new Vector2(-4f, 0f);
          RectTransform txt = Utils.FindChild(gameObject.transform, "leveltext").GetComponent<Text>().rectTransform;
          txt.anchoredPosition = new Vector2(txt.anchoredPosition.x, txt.anchoredPosition.y + 2f);
          // Remove currentlevel bar
          UnityEngine.Object.Destroy(Utils.FindChild(gameObject.transform, "currentlevel").GetComponent<GuiBar>().gameObject);

          __instance.m_elements.Add(gameObject);
        }
        float size = Mathf.Max(__instance.m_baseListSize, ((float)skillList.Count + paddingFix)* __instance.m_spacing);
        __instance.m_listRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        // Devs added this, but forgot to render it...
        __instance.m_totalSkillText.text = string.Concat(new string[]
        {
          "<color=orange>",
          player.GetSkills().GetTotalSkill().ToString("0"),
          "</color><color=white> / </color><color=orange>",
          player.GetSkills().GetTotalSkillCap().ToString("0"),
          "</color>"
        });
      }
    }
  }
  */
}
