using HarmonyLib;
using BetterUI.Patches;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterCookingStation
  {
    // [E] Cook item
    private static readonly string _cookItem = "[<color=yellow><b>$KEY_Use</b></color>] $piece_cstand_cook";
    // [1-8] Cook Item
    private static readonly string _selectItem = "[<color=yellow><b>1-8</b></color>] $piece_cstand_cook";
    private static readonly string overCookColor = "red";

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CookingStation), "GetHoverText")]
    private static bool GetHoverText(CookingStation __instance, ref string __result)
    {
      if (__instance.m_nview.IsOwner() && __instance.IsFireLit() && Main.timeLeftStyleCookingStation.Value != 0)
      {
        string cookingItems = "";
        int items = 0;

        for (int i = 0; i < __instance.m_slots.Length; i++)
        {
          __instance.GetSlot(i, out string text, out float num);
          if (text != "" && text != __instance.m_overCookedItem.name)
          {
            CookingStation.ItemConversion itemConversion = __instance.GetItemConversion(text);
            if (text != null)
            {
              items++;
              if (num > itemConversion.m_cookTime) // Item overCooking
              {
                string time = Main.timeLeftStyleCookingStation.Value == 1 ? $"{num / (itemConversion.m_cookTime * 2f):P0}" : Helpers.TimeString(itemConversion.m_cookTime * 2f - num);
                cookingItems += $"\n{__instance.m_overCookedItem.GetHoverName()}: <color={overCookColor}>{time}</color>";
              } else
              {
                string time = Main.timeLeftStyleCookingStation.Value == 1 ? $"{num / itemConversion.m_cookTime:P0}" : Helpers.TimeString(itemConversion.m_cookTime - num);
                cookingItems += $"\n{itemConversion.m_to.GetHoverName()}: {time}";
              }
            }
          }
        }
        if (items > 0)
        {
          __result = items >= __instance.m_slots.Length ? 
            Localization.instance.Localize($"{__instance.m_name}{cookingItems}") : 
            Localization.instance.Localize($"{__instance.m_name}\n{_cookItem}\n{_selectItem}{cookingItems}");
          return false; // Overwrite games default string
        }
      }
      return true;
    }
  }
}
