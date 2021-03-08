using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine.UI;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterSkillsDialog
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SkillsDialog), "Setup")]
    private static void ChangeGUI(ref SkillsDialog __instance, ref Player player)
    {
      __instance.gameObject.SetActive(true);
      if (Main.showCharacterXP.Value)
      {
        Utils.FindChild(__instance.transform, "topic").GetComponent<Text>().text = $"LV. {Patches.XP.level:0}";
      }
      if (!Main.customSkillUI.Value) return;

      Patches.SkillUI.UpdateDialog(__instance, player);
    }
  }
}
