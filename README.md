# eXtraTMP (XTMP)

eXtraTMP is a lightweight BepInEx plugin designed to seamlessly add custom fallback fonts to TextMeshPro (TMP).

## Usage Guide

### 1. Preparing Font Assets
1. Generate a font atlas using Unity's [Font Asset Creator](https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/FontAssetsCreator.html).
2. Package one or more font assets into a single file using the Unity [AssetBundle Browser](https://github.com/Unity-Technologies/AssetBundles-Browser).

### 2. Installation & Configuration
1. Place the generated AssetBundle file into the `BepInEx/plugins/XTMP/bundles` directory.
2. In the same directory, create a configuration file with the exact same name as your bundle, but with an `.ini` extension (e.g., if your bundle is `myfonts.bundle`, create `myfonts.ini`).
3. Define your fallback rules in the `.ini` file using the syntax detailed below. 

> **Note:** The plugin uses a simplified parser. While it resembles standard INI formatting, it does not support full INI syntax.

### Configuration Syntax

```ini
# GLOBAL FALLBACK
# Fonts listed without a section header are automatically added to the global fallback list in TMP_Settings.
Galmuri7

# DIRECT REPLACEMENT
# Completely replace an original font with a new one using the '->' operator.
# Syntax: [OriginalFontName -> ReplacementFontName]
[ExperimentIEatCrayons -> KCCKimHoon SDF]

# TARGETED FALLBACKS
# Specify an original font as a section header to assign fallbacks exclusively to that font.
[Retro Gaming]
NeoDunggeunmoPro

# If multiple fonts are listed under a section, they will be applied sequentially as fallbacks.
[LiberationSans SDF]
NotoSansKR SDF
NotoSansJP SDF
