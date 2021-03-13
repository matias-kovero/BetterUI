using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  public static class BetterSkills
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Skills), "RaiseSkill")]
    private static void Notification(ref Skills __instance, ref Skills.SkillType skillType, ref float factor)
    {
      if (Main.showXPNotifications.Value)
      {
        Skills.Skill skill = __instance.GetSkill(skillType);
        Patches.XPNotification.Show(skill, factor);
      }
    }

    [HarmonyPatch]
    static class Skill
    {
      [HarmonyPostfix]
      [HarmonyPatch(typeof(Skills.Skill), "Raise")]
      private static void CalculateXP(ref Skills __instance, float factor)
      {
        if (BetterHud._bar == null) return;

        Patches.XP.RaiseXP();
        BetterHud._bar.SetValue(Patches.XP.GetLevelPercentage());
      }
    }
  }
}
