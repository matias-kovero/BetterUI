using UnityEngine;

namespace BetterUI.Patches
{
  static class Compatibility
  {
    public static class QuickSlotsHotkeyBar
    {
      private static readonly string originalPath = "healthpanel/Health/QuickSlotsHotkeyBar";
      private static readonly string parent = "hudroot";
      public static bool isUsing = false;

      /// <summary>
      /// Remove from healthbar, and move up to hudroot.
      /// </summary>
      public static void Unanchor(Hud hud)
      {
        Transform parentTransform = Hud.instance.transform.Find(parent);
        Transform quickSlots = parentTransform.Find(originalPath);

        if (quickSlots)
        {
          isUsing = true;
          quickSlots.parent = hud.m_rootObject.transform;
          // No rotation for now, as objects are created and edited all the time on runtime...
          /*
          int rot = 90 - (Main.staminaBarRotation.Value / 90 % 4 * 90);
          Vector3 oldRot = quickSlots.localEulerAngles;
          quickSlots.localEulerAngles = new Vector3(0, 0, -rot);

          foreach (Transform child in quickSlots)
          {
            Helpers.DebugLine($"Editing: {child}.\n{child.localEulerAngles}\n{quickSlots.localEulerAngles}");
            child.localEulerAngles = oldRot;
          }
          */
          //quickSlots.SetParent(parentTransform, false);
        }
      }
    }
  }
}
