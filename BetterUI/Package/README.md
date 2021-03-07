
# BetterUI for Valheim
![HotBar](https://i.nyah.moe/R6KUG.png)  
This mod updates the game UI with few subtle edits.  
You are able to edit and select what updates you want to use. 

Feel free to check my other mod as well [First-Person Mod](https://valheim.thunderstore.io/package/Masa/FirstPerson/)
## Table of Contents
1. [Installation](#Installation-(manual))
2. [Preview](#Preview)
3. [Default Config](#Config)
4. [Changelog](#Changelog)

## Installation (manual)

If you are installing this manually, do the following _(You will need Bepinex installed)_

1. Extract the archive into a folder. **Do not extract into the game folder.**
2. Move the contents of `plugins` folder into `<GameDirectory>\Bepinex\plugins`.
3. Run the game, it will generate automatically an configuration file into `<GameDirectory>\Bepinex\config`

## Preview
![ItemUpgrade](https://i.nyah.moe/RwYtO.png)![ItemUpgrade2](https://i.nyah.moe/RwE8M.png)
![ItemTooltip](https://i.nyah.moe/RwGT3.png)![AllStats](https://i.nyah.moe/RwVMz.png)
![EnemyHUD](https://i.nyah.moe/R6PHf.png)  
![SkillGUI](https://i.nyah.moe/R602s.png)  


## Config
```
[HUD]

## Toggle the use of custom enemy hud
# Setting type: Boolean
# Default value: true
useCustomEnemyHud = true

## Select Text size on enemyHud
# Setting type: Int32
# Default value: 14
enemyHudTextSize = 14

## How far you will see enemy HP Bar
# Setting type: Single
# Default value: 20
MaxShowDistance = 40

[Item]

## Show colored durability bars
# Setting type: Boolean
# Default value: true
ShowDurabilityColor = true

## Show customized tooltips.
# Setting type: Boolean
# Default value: true
showCustomTooltips = false

## Scale item icon by this factor. Ex. 0.75 makes them 75% of original size
# Setting type: Single
# Default value: 0.75
ScaleSize = 0.7

[UI]

## Show Character XP Bar.
# Setting type: Boolean
# Default value: true
showCharacterXP = true

## Show when you gain xp from actions.
# Setting type: Boolean
# Default value: true
ShowXPNotifications = true

## Toggle the use of custom skills UI
# Setting type: Boolean
# Default value: true
useCustomSkillUI = true

## Select text size on skills UI
# Setting type: Int32
# Default value: 14
skillUITextSize = 24

## Toggle the visibility of custom info on character selection
# Setting type: Boolean
# Default value: true
showCustomCharInfo = true

## Show all item stats when mouse is hovered over armour amount.
# Setting type: Boolean
# Default value: true
showCombinedItemStats = false
```
## Changelog
#### 1.5.0
- Added custom tooltips
- Updated recipe information. Show clearly what stats are improving when upgrading.
- Show all active item stats by hovering over your armour amount.
#### 1.4.2
- Added skillUITextSize to config (Game still scales them)
- Padding fix on modded items quality stars
- Ability to toggle if chracter stats are visible 
#### 1.4.1
- Hotfix on config values
#### 1.4.0
- Added Character Lvls + XP Bar to track progress
- More options to config
- Bug fixes on spawned items breaking items UI
#### 1.3.1
- Fixed durability bars sometime showing as empty
- Small code optimization
- Updated Icon & README
#### 1.3.0
- Added custom enemy hud
- Editable enemy hud visibility range.
- Ability to scale item image sizes. _Defaulted to 75% of original size._
- **Removed First Person Camera** _(There is an seperate mod for that, few updates coming soon.)_
#### 1.2.0
- Changed item levels to display as stars
- Added Character stats to character selection screen
- Minor tweaks on skill UI.
#### 1.1.0
- Added Colored Durability Bars
- Added options to disable UI modifications
- Added config options to edit FPS Toggle and FOV

#### 1.0.1
- Initial Release under Valheim Mods
#### 1.0.0
- Initial Release, but went under the wrong section