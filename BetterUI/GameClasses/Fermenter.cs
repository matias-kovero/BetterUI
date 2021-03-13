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
      switch (__instance.GetStatus())
      {
        case Fermenter.Status.Fermenting:
          string contentName = __instance.GetContentName();
          if (__instance.m_exposed) // Why do we need to re-check? Ain't Fermenter.Status.Exposed enough? - Wack original code.
          {
            __result = Localization.instance.Localize(__instance.m_name + " ( " + contentName + ", $piece_fermenter_exposed )");
            return false;
          }
          string time = Main.timeLeftStyleFermenter.Value == 1 ? 
            $"{__instance.GetFermentationTime() / __instance.m_fermentationDuration:P0}" : 
            Patches.Helpers.TimeString(__instance.m_fermentationDuration - __instance.GetFermentationTime());

          __result = Localization.instance.Localize($"{contentName}\n$piece_fermenter_fermenting: {time}");
          return false;

        case Fermenter.Status.Ready:
          string contentName2 = __instance.GetContentName();
          __result = Localization.instance.Localize($"{__instance.m_name}, $piece_fermenter_ready \n{contentName2}\n[<color=yellow><b>$KEY_Use</b></color>] $piece_fermenter_tap");
          return false;

        default:
          return true;
      }
    }
  }
}
