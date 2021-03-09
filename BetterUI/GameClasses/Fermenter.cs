using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
      if (!PrivateArea.CheckAccess(__instance.transform.position, 0f, false))
      {
        __result = Localization.instance.Localize(__instance.m_name + "\n$piece_noaccess");
        return false;
      }
      switch (__instance.GetStatus())
      {
        case Fermenter.Status.Empty:
          __result = Localization.instance.Localize(__instance.m_name + " ( $piece_container_empty )\n[<color=yellow><b>$KEY_Use</b></color>] $piece_fermenter_add");
          return false;
        case Fermenter.Status.Fermenting:
          {
            string contentName = __instance.GetContentName();
            if (__instance.m_exposed)
            {
              __result = Localization.instance.Localize(__instance.m_name + " ( " + contentName + ", $piece_fermenter_exposed )");
              return false;
            }
            if (Main.timeLeftStyleFermenter.Value == 1)
            {
              double percentage = __instance.GetFermentationTime() / __instance.m_fermentationDuration * 100f;
              __result = Localization.instance.Localize($"$piece_fermenter_fermenting [{percentage:0}%]\n({contentName})");
              return false;
            }
            else if (Main.timeLeftStyleFermenter.Value == 2)
            {
              TimeSpan t = TimeSpan.FromSeconds(__instance.m_fermentationDuration - __instance.GetFermentationTime());
              string time = string.Format("{0:D2}m {1:D2}s", t.Minutes, t.Seconds);
              __result = Localization.instance.Localize($"{contentName}\n$piece_fermenter_fermenting: {time}");
              return false;
            }
            else
            {
              __result = Localization.instance.Localize(__instance.m_name + " ( " + contentName + ", $piece_fermenter_fermenting )");
              return false;
            }
          }
        case Fermenter.Status.Ready:
          {
            string contentName2 = __instance.GetContentName();
            __result = Localization.instance.Localize($"{__instance.m_name}, $piece_fermenter_ready \n({contentName2})\n[<color=yellow><b>$KEY_Use</b></color>] $piece_fermenter_tap");
            //__result = Localization.instance.Localize(__instance.m_name + " ( " + contentName2 + ", $piece_fermenter_ready )\n[<color=yellow><b>$KEY_Use</b></color>] $piece_fermenter_tap");
            return false;
          }
      }
      __result = __instance.m_name;
      // Fermenter ( Barley wine base: Fire resistance, Done )
      // [E] Tap
      return false; // Force to override games original function.
    }
  }
}
