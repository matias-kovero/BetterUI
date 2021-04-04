using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BetterUI.Patches;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  public static class BetterHud
  {
    private static Player _player = null;
    public static GuiBar _bar = null;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Hud), "Awake")]
    private static void Awake(ref Hud __instance)
    {
      if (Main.useCustomHealthBar.Value) CustomElements.HealthBar.Create();
      if (Main.useCustomStaminaBar.Value) CustomElements.StaminaBar.Create();
      if (Main.useCustomFoodBar.Value) CustomElements.FoodBar.Create();

      if (Main.showCharacterXP.Value && _bar == null) _bar = XPBar.Awake(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Hud), "Update")]
    private static void UpdateXPBar(Hud __instance)
    {
      Player localPlayer = Player.m_localPlayer;
      if (!Main.showCharacterXP.Value) return;
      if (_player == null && localPlayer != null)
      {
        _player = localPlayer;
        Patches.XP.Awake(localPlayer);
        _bar.SetValue(Patches.XP.GetLevelPercentage());
      }

      // For XP Bar Debug
      /*
      if (_bar != null)
      {
        
        if (_bar.m_value >= 1f) _bar.m_value = 0f;
        _bar.SetValue(_bar.m_value + 0.001f);
      }
      */
    }
  }
}
