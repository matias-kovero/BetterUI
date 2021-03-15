using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterFermenter
  {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Fermenter), "GetHoverText")]
    private static bool GetHoverText(ref Fermenter __instance, ref string __result)
    {
      if (Main.timeLeftStyleFermenter.Value == 0) return true;

      if (!PrivateArea.CheckAccess(__instance.transform.position, 0f, false))
      {
        __result = Localization.instance.Localize(__instance.m_name + "\n$piece_noaccess");
        return false;
      }

      return Patches.HoverText.PatchFermenter(__instance, ref __result);
    }
  }
}
