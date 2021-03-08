using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterEnemyHud
  {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyHud), "Awake")]
    private static void PatchDefaults(ref EnemyHud __instance)
    {
      __instance.m_maxShowDistance = Mathf.Min(Mathf.Abs(Main.maxShowDistance.Value), Patches.CustomEnemyHud.maxDrawDistance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyHud), "ShowHud")]
    private static void PatchName(ref EnemyHud __instance, ref Character c)
    {
      if (!Main.customEnemyHud.Value) return;
      Patches.CustomEnemyHud.AddLvlToName(__instance, c);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyHud), "UpdateHuds")]
    private static void UpdateHP(ref EnemyHud __instance, Player player, float dt)
    {
      if (!Main.customEnemyHud.Value) return;
      Patches.CustomEnemyHud.UpdateHPText(__instance);
    }
  }
}
