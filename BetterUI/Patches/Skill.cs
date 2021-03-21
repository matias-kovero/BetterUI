using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
  class XPNotification
  {
    public static void Show(Skills.Skill skill, float factor)
    {
      string notif_str = $"$skill_{ skill.m_info.m_skill.ToString().ToLower()}: {skill.GetLevelPercentage():P2}";

      if (Main.extendedXPNotification.Value)
      {
        float acc = (float)Math.Round(skill.m_accumulator * 100f) / 100f;
        float max = (float)Math.Round(skill.GetNextLevelRequirement() * 100f) / 100f;
        notif_str += $" (+{ skill.m_info.m_increseStep* factor})\n[{acc}/{max}]";
      }

      string str = Localization.instance.Localize(notif_str);
      MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, $"<size={Main.notificationTextSize.Value}>{str}</size>");
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
        gameObject.GetComponentInChildren<UITooltip>().m_text = skill.m_info.m_description;

        Utils.FindChild(gameObject.transform, "icon").GetComponent<Image>().sprite = skill.m_info.m_icon;
        Utils.FindChild(gameObject.transform, "name").GetComponent<Text>().text = Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower() + $"\n<size={Main.skillUITextSize.Value - 2}>Lvl: {(int)skill.m_level}</size>");
        Utils.FindChild(gameObject.transform, "name").GetComponent<Text>().fontSize = Main.skillUITextSize.Value;
        Utils.FindChild(gameObject.transform, "leveltext").GetComponent<Text>().text = $"<size={Main.skillUITextSize.Value-4}>{acc} ({skill.GetLevelPercentage() * 100f:0.##}%)</size>";
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
}
