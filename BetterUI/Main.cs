using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Reflection;

namespace BetterUI
{
  [BepInPlugin(GUID, MODNAME, VERSION)]
  public class Main : BaseUnityPlugin
  {
    #region[Declarations]

    public const string
      MODNAME = "BetterUI",
      AUTHOR = "MK",
      GUID = AUTHOR + "_" + MODNAME,
      VERSION = "1.5.0";

    internal readonly ManualLogSource log;
    internal readonly Harmony harmony;
    internal readonly Assembly assembly;
    public readonly string modFolder;

    public static ConfigEntry<bool> showXPNotifications;
    public static ConfigEntry<bool> showCharacterXP;
    public static ConfigEntry<bool> showDurabilityColor;
    public static ConfigEntry<float> iconScaleSize;
    public static ConfigEntry<bool> customEnemyHud;
    public static ConfigEntry<bool> customSkillUI;
    public static ConfigEntry<float> maxShowDistance;
    public static ConfigEntry<int> enemyHudTextSize;
    public static ConfigEntry<int> skillUITextSize;
    public static ConfigEntry<bool> showCustomCharInfo;
    public static ConfigEntry<bool> showCustomTooltips;
    public static ConfigEntry<bool> showCombinedItemStats;
    //public static ConfigEntry<string> durabilityColorMode;

    #endregion


    public Main()
    {
      log = Logger;
      harmony = new Harmony(GUID);
      assembly = Assembly.GetExecutingAssembly();
      //modFolder = Path.GetDirectoryName(assembly.Location);
    }
    public void Awake()
    {
      showCharacterXP = Config.Bind("UI", "showCharacterXP", true, "Show Character XP Bar.");
      showXPNotifications = Config.Bind("UI", "ShowXPNotifications", true, "Show when you gain xp from actions.");
      customSkillUI = Config.Bind("UI", "useCustomSkillUI", true, "Toggle the use of custom skills UI");
      skillUITextSize = Config.Bind("UI", "skillUITextSize", 14, "Select text size on skills UI");
      showCustomCharInfo = Config.Bind("UI", "showCustomCharInfo", true, "Toggle the visibility of custom info on character selection");
      showCombinedItemStats = Config.Bind("UI", "showCombinedItemStats", true, "Show all item stats when mouse is hovered over armour amount.");
      showDurabilityColor = Config.Bind("Item", "ShowDurabilityColor", true, "Show colored durability bars");
      showCustomTooltips = Config.Bind("Item", "showCustomTooltips", true, "Show customized tooltips.");
      iconScaleSize = Config.Bind("Item", "ScaleSize", 0.75f, "Scale item icon by this factor. Ex. 0.75 makes them 75% of original size");
      customEnemyHud = Config.Bind("HUD", "useCustomEnemyHud", true, "Toggle the use of custom enemy hud");
      enemyHudTextSize = Config.Bind("HUD", "enemyHudTextSize", 14, "Select Text size on enemyHud");
      maxShowDistance = Config.Bind("HUD", "MaxShowDistance", 20f, "How far you will see enemy HP Bar");
    }
    public void Start()
    {
      harmony.PatchAll(assembly);
    }
    public void OnDestroy()
    {
      harmony?.UnpatchAll();
    }
  }
}
