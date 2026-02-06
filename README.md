# eXtraTMP

XTMP is a simple BepInEx plugin to add fallback fonts to TextMeshPro.

## Creating Font Assets for XTMP

1. Create a font atlas using Unity's [Font Asset Creator](https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/FontAssetsCreator.html).
2. Pack single or multiple assets into one file using [AssetBundles](https://github.com/Unity-Technologies/AssetBundles-Browser).
3. Place the bundle file in the `BepInEx/plugins/XTMP/bundles` folder and create an `ini` file with the same name as the bundle.
4. Configure the fallback fonts in the `ini` file using the syntax below. (This uses a syntax similar to INI, but does not support full INI syntax.)

```ini
# If no section is defined, the global table in TMP_Settings is used.
NeoDunggeunmo SDF

# Use a section to specify the name of the font you want to replace.
[Retro GamingPix]
NeoDunggeunmo

[Retro Gaming SDF]
# If multiple fonts are listed, they are used sequentially.
NeoDunggeunmo SDF
NeoDunggeunmo
```
