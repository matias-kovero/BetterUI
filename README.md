![BetterUI Logo](https://github.com/matias-kovero/BetterUI/blob/main/logo.png)
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
 
 You should now successfully build the project 🎉
 
 To view the actual game code, download [dnSpy](https://github.com/dnSpy/dnSpy/releases/tag/v6.1.8) and open `assembly_valheim.dll` with it.  
 The file is located in `<ValheimGameDirectory>\valheim_Data\Managed\assembly_valheim.dll`
