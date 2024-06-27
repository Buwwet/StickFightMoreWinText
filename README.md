# Customizable Victory Texts
A mod for Stick Fight The Game using Bepinex LINK that allows for adding new win texts and even completely disabling the built-in ones.
It also boasts the ability to add player specific win texts.

Thanks to not being a core-mod, this mod is compatible with other mods.

## Customization

To add or remove victory texts, simply head to `StickFightTheGame/BepInEx/plugins/MoreWinText/texts/` and modify the text files found there, changes will appear on game restart.

NOTE: By default, vanilla win texts get disable so that you can verify if the plugin was succesfully installed, to re-enable them you have to set the field `Disable built-in win texts` in `StickFightTheGame/BepInEx/plugins/MoreWinText/config.cfg` to false


# Instalation
## Installing BepinEx
### Installing BepinEx on Windows
Download the latest release from https://github.com/BepInEx/BepInEx/releases for your Windows version (either x64 for 64 bit or x86 for 32 bit) 

Drag the zip file's contents into the root Stick Fight The Game directory, you can get to this location by right-clicking the game on Steam > `Manage > Browse local files`.

Run the game once, if new directories were created in `StickFightTheGame/BepInEx`, it worked!

### Instaling BepinEx on Linux
This part of the installation is only for Linux users, there are a few more steps that you have to take.

Set the launch options of Stick Fight The Game to `PROTON_USE_WINED3D11=1 WINEDLLOVERRIDES="winhttp=n,b" %command%`

Download the latest release from https://github.com/BepInEx/BepInEx/releases for windows x86 and drag its contents to the Stick Fight The Game directory that contains the `.exe` file, it should be the root one.

Now we need to modify the Wineprefix so that the game gets patched by the `.dll` that BepInEx suppies. To do this we need a tool like [protontricks](https://github.com/Matoking/protontricks) to modify the prefix to add an exception for that `.dll`

Run `protontricks --gui` and navigate the following menu > Stick Fight The Game > Select the default wineprefix > Run winecfg > Libraries |
Now add an override for the `winhttp` library. Hit apply and you can now close protontricks.

Nice, so now Wine will allow us to load this dll, but now we have to fight Proton. Proton stores all the necesary and unnecesary `.dll` files for each game, so we have to head to where they are being stored and replace `winhtpp` in both `system32` and `syswow64` with a symbolic link that points to BepInEx's dll.

Go to `steamapps(depends on your installation)/compatdata/674940/pfx/drive_c/windows/system32` and delete `winhttp.dll` and replace it with a syslink to the dll we have in the game folder. I did it like this: `ln -s ./../../../../../../common/StickFightTheGame/winhttp.dll ./`

Then go to `steamapps(depends on your installation)/compatdata/674940/pfx/drive_c/windows/syswow64` and do the same as the previous step, just in case.

Finally, if you run the game, it should hopefully get patched by BepInEx and create some folders in `StickFightTheGame/BepInEx`

## Mod installation
Now that you have BepInEx working, drag the mod's folder to `StickFightTheGame/BepInEx/plugins/` and you're set!

The directory structure should look something like this:
```
BepInEx/
├── cache
│   └── ...
├── config
│   └── ...
├── core
│   └── ...
├── LogOutput.log
├── patchers
└── *plugins*
    ├── **MoreWinText**
    │   ├── MoreVictoryText.dll
    │   ├── config.cfg
    │   └── *texts*
    │       ├── blue.txt
    │       ├── general.txt
    │       ├── green.txt
    │       ├── red.txt
    │       └── yellow.txt
```

Run the game and kill your friends to see if it worked. By default the mod disables vanilla win texts so that you can prove that it got installed correctly. You should revert that in the config.

# Compiling
Thanks for being interested in my mod! Here's all the requirements you need to build this.

## Requirements
* .NET 
* You have to copy a few `.dlls` from the game into `libs/`, these are: `Assembly-CSharp.dll`, `UnityEngine.UI.dll` and `TextMeshPro-1.0.55.56.0b9.dll`. They can be found in `StickFightTheGame/StickFight_Data/Managed`


## Building

Just `dotnet build` and you're done, the mod's `.dll` can now be found in `bin/Debug/net46/MoreVictoryText.dll`. I usually store a symbolic link in the game folder so I don't have to move it every time I build.
