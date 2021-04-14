using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

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
      VERSION = "2.0.0";

    internal readonly ManualLogSource log;
    internal readonly Harmony harmony;
    internal readonly Assembly assembly;
    public readonly string modFolder;

    public static readonly bool isDebug = true;

    // Custom Elements
    public static ConfigEntry<bool> useCustomHealthBar;
    public static ConfigEntry<bool> useCustomStaminaBar;
    public static ConfigEntry<bool> useCustomFoodBar;

    // HoverTexts
    public static ConfigEntry<int> timeLeftStyleFermenter;
    public static ConfigEntry<int> timeLeftStylePlant;
    public static ConfigEntry<int> timeLeftStyleCookingStation;
    public static ConfigEntry<int> chestHasRoomStyle;

    // Settings
    public static ConfigEntry<bool> useCustomHud;
    public static ConfigEntry<KeyCode> toggleEditMode;
    public static ConfigEntry<KeyCode> modKeyPrimary;
    public static ConfigEntry<KeyCode> modKeySecondary;
    public static ConfigEntry<int> colorMode;
    public static ConfigEntry<bool> showDurabilityColor;
    public static ConfigEntry<bool> showCharacterXP;
    public static ConfigEntry<bool> showItemStars;
    public static ConfigEntry<bool> customEnemyHud;
    public static ConfigEntry<bool> hideEnemyHPText;
    public static ConfigEntry<bool> showXPNotifications;
    public static ConfigEntry<bool> customSkillUI;
    public static ConfigEntry<bool> showCustomCharInfo;
    public static ConfigEntry<bool> showCustomTooltips;
    public static ConfigEntry<bool> showCombinedItemStats;

    // UI Edits
    public static ConfigEntry<float> iconScaleSize;
    public static ConfigEntry<float> mapPinScaleSize;
    public static ConfigEntry<int> notificationTextSize;
    public static ConfigEntry<bool> extendedXPNotification;
    public static ConfigEntry<int> enemyLvlStyle;
    public static ConfigEntry<int> enemyHudTextSize;
    public static ConfigEntry<float> maxShowDistance;
    public static ConfigEntry<int> skillUITextSize;
    public static ConfigEntry<int> healthBarRotation;
    public static ConfigEntry<int> staminaBarRotation;
    public static ConfigEntry<int> foodBarRotation;

    // xUIData
    public static ConfigEntry<string> uiData;
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
      /* =======================
       *        Settings
       * =======================
       */
      useCustomHud = Config.Bind("Settings",
        nameof(useCustomHud),
        true,
        "Toggle whether to use custom huds or not."
      );

      toggleEditMode = Config.Bind("Settings",
        nameof(toggleEditMode),
        KeyCode.F7,
        "Toggle hud editing mode."
      );

      modKeyPrimary = Config.Bind("Settings",
        nameof(modKeyPrimary),
        KeyCode.Mouse0,
        "Button needed to hold down to change HUD position. Check values: https://docs.unity3d.com/ScriptReference/KeyCode.html"
      );

      modKeySecondary = Config.Bind("Settings",
        nameof(modKeySecondary),
        KeyCode.LeftControl,
        "Button needed to hold down to change element dimensions. Accepted Values: https://docs.unity3d.com/ScriptReference/KeyCode.html"
      );

      colorMode = Config.Bind("Settings", 
        nameof(colorMode), 
        0, 
        "Change colorMode. Options: 0=Normal, 1=Protanopia"
      );

      showDurabilityColor = Config.Bind("Settings", 
        nameof(showDurabilityColor), 
        true, 
        "Show colored durability bars"
      );

      showCharacterXP = Config.Bind("Settings", 
        nameof(showCharacterXP), 
        true, 
        "Show Character XP Bar."
      );

      showItemStars = Config.Bind("Settings",
        nameof(showItemStars), 
        true, 
        "Show item quality as stars"
      );

      customEnemyHud = Config.Bind("Settings",
        nameof(customEnemyHud), 
        true, 
        "Toggle the use of custom enemy hud"
      );

      hideEnemyHPText = Config.Bind("Settings",
        nameof(hideEnemyHPText), 
        false, 
        "Toggle if you want to hide the text with HP amount"
      );

      showXPNotifications = Config.Bind("Settings",
        nameof(showXPNotifications), 
        true, 
        "Show when you gain xp from actions."
      );

      customSkillUI = Config.Bind("Settings",
        nameof(customSkillUI), 
        true, 
        "Toggle the use of custom skills UI"
      );

      showCustomCharInfo = Config.Bind("Settings",
        nameof(showCustomCharInfo), 
        true, 
        "Toggle the visibility of custom info on character selection"
      );

      showCustomTooltips = Config.Bind("Settings",
        nameof(showCustomTooltips), 
        true, 
        "Show customized tooltips."
      );

      showCombinedItemStats = Config.Bind("Settings",
        nameof(showCombinedItemStats), 
        true, 
        "Show all item stats when mouse is hovered over armour amount."
      );

      /* =======================
       *        UI Edits
       * =======================
       */
      iconScaleSize = Config.Bind("UI Edits",
        nameof(iconScaleSize), 
        0.75f, 
        "Scale item icon by this factor. Ex. 0.75 makes them 75% of original size"
      );

      mapPinScaleSize = Config.Bind("UI Edits",
        nameof(mapPinScaleSize), 
        1f, 
        "Scale map pins by this factor. Ex. 1.5 makes the 150% of original size."
      );

      notificationTextSize = Config.Bind("UI Edits",
        nameof(notificationTextSize), 
        14, 
        "Edit XP notification font size."
      );

      extendedXPNotification = Config.Bind("UI Edits",
        nameof(extendedXPNotification), 
        true, 
        "Extend notification with: (xp gained) [current/overall xp]"
      );

      enemyLvlStyle = Config.Bind("UI Edits",
        nameof(enemyLvlStyle), 
        1, 
        "Choose how enemy lvl is shown. 0 = Default(stars) | 1 = Prefix before name (Lv. 1) | 2 = Both"
      );

      enemyHudTextSize = Config.Bind("UI Edits",
        nameof(enemyHudTextSize), 
        14, 
        "Select Text size on enemyHud"
      );

      maxShowDistance = Config.Bind("UI Edits",
        nameof(maxShowDistance), 
        1f, 
        "How far you will see enemy HP Bar. This is an multiplier, 1 = game default. 2 = 2x default"
      );

      skillUITextSize = Config.Bind("UI Edits",
        nameof(skillUITextSize), 
        14, 
        "Select text size on skills UI"
      );

      healthBarRotation = Config.Bind("UI Edits",
        nameof(healthBarRotation),
        90,
        "Rotate healthbar in degrees"
      );

      staminaBarRotation = Config.Bind("UI Edits",
        nameof(staminaBarRotation),
        90,
        "Rotate staminabar in degrees"
      );

      foodBarRotation = Config.Bind("UI Edits",
        nameof(foodBarRotation),
        90,
        "Rotate foodbar in degrees"
      );

      /* =======================
       *     Hover Texts
       * =======================
       */
      timeLeftStyleFermenter = Config.Bind("Hover Text",
        nameof(timeLeftStyleFermenter), 
        2, 
        "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left"
      );

      timeLeftStylePlant = Config.Bind("Hover Text",
        nameof(timeLeftStylePlant), 
        2, 
        "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left"
      );

      timeLeftStyleCookingStation = Config.Bind("Hover Text",
        nameof(timeLeftStyleCookingStation), 
        2, 
        "Select duration display. 0 = Default, 1= % Done, 2 = min:sec left"
      );

      chestHasRoomStyle = Config.Bind("Hover Text",
        nameof(chestHasRoomStyle), 
        2, 
        "Select how chest emptyness is displayed. 0 = Default | 1 = % | 2 = items / max_room. | 3 = free slots "
      );


      /* =======================
       *     Custom Elements
       * =======================
       */
      useCustomHealthBar = Config.Bind("CustomElements",
        nameof(useCustomHealthBar),
        true,
        "Select if you want to use an custom HP Bar."
      );
      useCustomStaminaBar = Config.Bind("CustomElements",
        nameof(useCustomStaminaBar),
        true,
        "Select if you want to use an custom Stamina Bar."
      );
      useCustomFoodBar = Config.Bind("CustomElements",
        nameof(useCustomFoodBar),
        true,
        "Select if you want to use an custom Food Bar."
      );

      /* =======================
       *         xDataUI
       * =======================
       */
      uiData = Config.Bind("xDataUI",
        nameof(uiData),
        "none",
        "This is your customized UI info. (Edit to none, if having issues with UI)"
      );
      /*
      showXPNotifications = Config.Bind("UI", "ShowXPNotifications", true, "Show when you gain xp from actions.");
      extendedXPNotification = Config.Bind("UI", "extendedXPNotification", true, "Extend notification with: (xp gained) [current/overall xp]");
      notificationTextSize = Config.Bind("UI", "notificationTextSize", 14, "Edit XP notification font size.");
      customSkillUI = Config.Bind("UI", "useCustomSkillUI", true, "Toggle the use of custom skills UI");
      skillUITextSize = Config.Bind("UI", "skillUITextSize", 14, "Select text size on skills UI");
      showCustomCharInfo = Config.Bind("UI", "showCustomCharInfo", true, "Toggle the visibility of custom info on character selection");
      showCombinedItemStats = Config.Bind("UI", "showCombinedItemStats", true, "Show all item stats when mouse is hovered over armour amount.");
      timeLeftStyleFermenter = Config.Bind("UI", "timeLeftStyleFermenter", 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");
      timeLeftStylePlant = Config.Bind("UI", "timeLeftStylePlant", 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");
      timeLeftStyleCookingStation = Config.Bind("UI", "timeLeftStyleCookingStation", 2, "Select duration display. 0 = Default, 1= % Done, 2 = min:sec left");
      chestHasRoomStyle = Config.Bind("UI", "chestHasRoomStyle", 2, "Select how chest emptyness is displayed. 0 = Default | 1 = % | 2 = items / max_room. | 3 = free slots ");

      showDurabilityColor = Config.Bind("Item", "ShowDurabilityColor", true, "Show colored durability bars");
      showItemStars = Config.Bind("Item", "showItemStars", true, "Show item quality as stars");
      showCustomTooltips = Config.Bind("Item", "showCustomTooltips", true, "Show customized tooltips.");
      iconScaleSize = Config.Bind("Item", "ScaleSize", 0.75f, "Scale item icon by this factor. Ex. 0.75 makes them 75% of original size");

      customEnemyHud = Config.Bind("HUD", "useCustomEnemyHud", true, "Toggle the use of custom enemy hud");
      hideEnemyHPText = Config.Bind("HUD", "hideEnemyHPText", false, "Toggle if you want to hide the text with HP amount");
      enemyLvlStyle = Config.Bind("HUD", "enemyLvlStyle", 1, "Choose how enemy lvl is shown. 0 = Default(stars) | 1 = Prefix before name (Lv. 1) | 2 = Both");
      enemyHudTextSize = Config.Bind("HUD", "enemyHudTextSize", 14, "Select Text size on enemyHud");
      maxShowDistance = Config.Bind("HUD", "MaxShowDistance", 1f, "How far you will see enemy HP Bar. This is an multiplier, 1 = game default. 2 = 2x default");
      mapPinScaleSize = Config.Bind("HUD", "mapPinSize", 1f, "Scale map pins by this factor. Ex. 1.5 makes the 150% of original size.");
      */
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
