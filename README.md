[<p align="center"><img width="600" src="/logo.png"></p>](/logo.png) 

![Version](https://img.shields.io/endpoint?color=success&label=Version&style=flat-square&url=https%3A%2F%2Fvalheim-modtracker.vercel.app%2Fnexusmods%2F189%2Fversion)
[![](https://img.shields.io/endpoint?label=TS%20Downloads&style=flat-square&url=https%3A%2F%2Fvalheim-modtracker.vercel.app%2Fthunderstore%2FMasa%2FBetterUI%2Fdownloads)](https://valheim.thunderstore.io/package/Masa/BetterUI/)
[![](https://img.shields.io/endpoint?label=TS%20%F0%9F%91%8D&style=flat-square&url=https%3A%2F%2Fvalheim-modtracker.vercel.app%2Fthunderstore%2FMasa%2FBetterUI%2Fratings)](https://valheim.thunderstore.io/package/Masa/BetterUI/)
[![](https://img.shields.io/endpoint?label=Nexus%20endorsed&style=flat-square&url=https%3A%2F%2Fvalheim-modtracker.vercel.app%2Fnexusmods%2F189%2Fendorsed)](https://www.nexusmods.com/valheim/mods/189)
[![](https://img.shields.io/endpoint?label=Nexus%20views&style=flat-square&url=https%3A%2F%2Fvalheim-modtracker.vercel.app%2Fnexusmods%2F189%2Fviews)](https://www.nexusmods.com/valheim/mods/189)
# BetterUI 
The main idea behind *BetterUI* is to make the interface of [Valheim](https://www.valheimgame.com/) more pleasant and easier to understand.  
I'll add slowly new additions to the mod - and fix bugs that users report.

## Where to download 
[Thunderstore](https://valheim.thunderstore.io/package/Masa/BetterUI/)  _*suggesting to use their mod manager for an quick & easy install_ ([r2modman](https://valheim.thunderstore.io/package/ebkr/r2modman/))

[NexusMods](https://www.nexusmods.com/valheim/mods/189)

## Project Structure
[/](/BetterUI/) - Project root  
[/GameClasses](/BetterUI/GameClasses) - HarmonyPatches under the original game classes.  
[/Patches](/BetterUI/Patches) - Supporting functions for specific patches.  
[/Package](/BetterUI/Package) - Package for Thunderstore upload.  

## Developing environment
#### BepInEx
 - Download and install [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) *(this is an Valheim specific pack)*
 - Follow the information under manual install
#### BepInEx Publicizer
 - To get access on games private functions, you need the publicized assembly files.
 - Install an assembly publicizer for BepInEx from:  https://github.com/MrPurple6411/Bepinex-Tools/releases/tag/1.0.0-Publicizer
 - The `Bepinex-Publicizer` folder from the `.zip` should be placed under `<ValheimGameDirectory>\BepInEx\plugins`
 - Run the game once, BepInEx console should pop-up. At the background, BepInEx Publicizer will create assemblies  
 under `<ValheimGameDirectory>\valheim_Data\Managed\publicized_assemblies`
 
 You should now successfully build this project 🎉
 
 ## Additional info
 To view the actual game code, download [dnSpy](https://github.com/dnSpy/dnSpy/releases/tag/v6.1.8) and open `assembly_valheim.dll` with it.  
 The file is located under `<ValheimGameDirectory>\valheim_Data\Managed\`
 
 This mod is created using [HarmonyX](https://github.com/BepInEx/HarmonyX), info about their syntax is found [here](https://harmony.pardeike.net/articles/patching.html).
