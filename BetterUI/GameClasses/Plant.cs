using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterPlant
  {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Plant), "GetHoverText")]
    private static bool GetHoverText(Plant __instance, ref string __result)
    {
			if (Main.timeLeftStylePlant.Value == 0) return true;

			switch (__instance.m_status)
      {
				case Plant.Status.Healthy:
					string time = Main.timeLeftStylePlant.Value == 1 ?
						$"{__instance.TimeSincePlanted() / __instance.GetGrowTime():P0}" :
						Patches.Helpers.TimeString(__instance.GetGrowTime() - __instance.TimeSincePlanted());

					__result = Localization.instance.Localize($"{__instance.m_name}\n{time}");
					return false;

				default:
					return true;
      }
    }
  }
}