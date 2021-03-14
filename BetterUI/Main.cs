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
      VERSION = "1.5.1";

    internal readonly ManualLogSource log;
    internal readonly Harmony harmony;
    internal readonly Assembly assembly;
    public readonly string modFolder;

    public static ConfigEntry<int> colorMode;
    public static ConfigEntry<bool> showXPNotifications;
    public static ConfigEntry<bool> showCharacterXP;
    public static ConfigEntry<bool> showDurabilityColor;
    public static ConfigEntry<bool> showItemStars;
    public static ConfigEntry<float> iconScaleSize;
    public static ConfigEntry<bool> customEnemyHud;
    public static ConfigEntry<int> enemyLvlStyle;
    public static ConfigEntry<bool> customSkillUI;
    public static ConfigEntry<float> maxShowDistance;
    public static ConfigEntry<int> enemyHudTextSize;
    public static ConfigEntry<int> skillUITextSize;
    public static ConfigEntry<bool> showCustomCharInfo;
    public static ConfigEntry<bool> showCustomTooltips;
    public static ConfigEntry<bool> showCombinedItemStats;
    public static ConfigEntry<int> timeLeftStyleFermenter;
    public static ConfigEntry<int> timeLeftStylePlant;
    public static ConfigEntry<int> timeLeftStyleCookingStation;
    public static ConfigEntry<float> mapPinScaleSize;
    public static ConfigEntry<int> chestHasRoomStyle;

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
      colorMode = Config.Bind("Settings", "colorMode", 0, "Change colorMode. Options: 0=Normal, 1=Protanopia ");

      showCharacterXP = Config.Bind("UI", "showCharacterXP", true, "Show Character XP Bar.");
      showXPNotifications = Config.Bind("UI", "ShowXPNotifications", true, "Show when you gain xp from actions.");
      customSkillUI = Config.Bind("UI", "useCustomSkillUI", true, "Toggle the use of custom skills UI");
      skillUITextSize = Config.Bind("UI", "skillUITextSize", 14, "Select text size on skills UI");
      showCustomCharInfo = Config.Bind("UI", "showCustomCharInfo", true, "Toggle the visibility of custom info on character selection");
      showCombinedItemStats = Config.Bind("UI", "showCombinedItemStats", true, "Show all item stats when mouse is hovered over armour amount.");
      timeLeftStyleFermenter = Config.Bind("UI", "timeLeftStyleFermenter", 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");
      timeLeftStylePlant = Config.Bind("UI", "timeLeftStylePlant", 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");
      timeLeftStyleCookingStation = Config.Bind("UI", "timeLeftStyleCookingStation", 2, "Select duration display. 0 = Default, 1= % Done, 2 = min:sec left");
      chestHasRoomStyle = Config.Bind("UI", "chestHasRoomStyle", 2, "Select how chest emptyness is displayed. 0 = Default | 1 = % | 2 = items / max_room. ");

      showDurabilityColor = Config.Bind("Item", "ShowDurabilityColor", true, "Show colored durability bars");
      showItemStars = Config.Bind("Item", "showItemStars", true, "Show item quality as stars");
      showCustomTooltips = Config.Bind("Item", "showCustomTooltips", true, "Show customized tooltips.");
      iconScaleSize = Config.Bind("Item", "ScaleSize", 0.75f, "Scale item icon by this factor. Ex. 0.75 makes them 75% of original size");

      customEnemyHud = Config.Bind("HUD", "useCustomEnemyHud", true, "Toggle the use of custom enemy hud");
      enemyLvlStyle = Config.Bind("HUD", "enemyLvlStyle", 1, "Choose how enemy lvl is shown. 0 = Default(stars) | 1 = Prefix before name (Lv. 1) | 2 = Both");
      enemyHudTextSize = Config.Bind("HUD", "enemyHudTextSize", 14, "Select Text size on enemyHud");
      maxShowDistance = Config.Bind("HUD", "MaxShowDistance", 1f, "How far you will see enemy HP Bar. This is an multiplier, 1 = game default. 2 = 2x default");
      mapPinScaleSize = Config.Bind("HUD", "mapPinSize", 1f, "Scale map pins by this factor. Ex. 1.5 makes the 150% of original size.");
    }
    public void Start()
    {
      harmony.PatchAll(assembly);
    }
    /*
    public void OnDestroy()
    {
      harmony?.UnpatchAll();
    }
    */
  }
}
