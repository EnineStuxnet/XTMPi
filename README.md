# eXtraTMP (XTMP)

**eXtraTMP** is a simple BepInEx plugin that allows you to add fallback fonts to existing fonts or configure alternative font replacements.

## Usage Guide

### 1. Preparing Fonts or Font Assets

Fonts can be loaded from either TrueType or AssetBundles.

#### Loading from TrueType Fonts
1. Place your font file (`*.ttf`) into the `BepInEx/plugins/XTMP` directory.
2. Specify the font in the configuration file using its exact filename.

#### Loading from AssetBundles
1. [Create a TextMeshPro asset within Unity](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.2/manual/FontAssetsCreator.html#creating-a-font-asset).
2. Build the asset into a bundle using the [AssetBundles-Browser](https://github.com/Unity-Technologies/AssetBundles-Browser).
3. Place the bundle file (either with no extension or a `*.bundle`) into the `BepInEx/plugins/XTMP` directory.

### 2. Creating the Configuration File

1. Write the configuration file by referring to the [Configuration Syntax](#configuration-syntax) section below.
2. Place your completed configuration file (`*.ini`) into the `BepInEx/plugins/XTMP` directory.

### 3. Directory Structure

Once everything is set up, your `BepInEx/plugins/XTMP` directory should look like the following structure:

```text
BepInEx/plugins/XTMP/
├── bundles/
│   ├── ko-KR.bundle
│   ├── ko-KR.bundle.ini
├── fonts/
│   ├── ko-KR/
│   │   ├── D2Coding.ttf
│   │   ├── Galmuri7.ttf
│   │   └── NotoSansKR.ttf
│   └── ko-KR.bundle.ini
└── XTMP.dll
```

## Configuration Syntax

> **Note:** The configuration file format is similar to INI, but it does not support actual, standard INI syntax.

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
```
