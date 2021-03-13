using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  public static class BetterFejdStartup
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(FejdStartup), "SetupGui")]
    private static void AddWaterMark(ref FejdStartup __instance)
    {
      Patches.CustomWatermark.Apply(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(FejdStartup), "UpdateCharacterList")]
    private static void ShowCharacterStats(ref FejdStartup __instance)
    {
      if (Main.showCustomCharInfo.Value)
      {
        Patches.CharacterStats.Show(__instance);
      }
    }
  }
}
