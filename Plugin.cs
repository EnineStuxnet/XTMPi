using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.LowLevel;

namespace XTMP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static Dictionary<string, TMP_FontAsset> Fonts = [];
    internal static Dictionary<string, string> ReplaceFonts = [];
    internal static Dictionary<string, List<string>> FallbackFonts = [];

    static readonly string[] BundleExtensions = ["", ".bundle"];
    static readonly string ResourceDir = Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_NAME);

    void Awake()
    {
        Logger = base.Logger;

        var enumerator = Directory.EnumerateFiles(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_NAME), "*", SearchOption.AllDirectories);

        foreach (var path in enumerator)
        {
            try
            {
                switch (Path.GetExtension(path).ToLower())
                {
                    case "":
                    case ".bundle":
                        LoadBundle(path);
                        break;
                    case ".ttf":
                        LoadFont(path);
                        break;
                    case ".ini":
                        LoadMap(path);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to process {path}: {e}");
            }
        }

        SceneManager.sceneLoaded += (_, _) => ApplyFonts();
    }

    public static void LoadBundle(string path)
    {
        var bundle = AssetBundle.LoadFromFile(path);

        foreach (var font in bundle.LoadAllAssets<TMP_FontAsset>())
        {
            Fonts[font.name] = font;
            DontDestroyOnLoad(font);
            Logger.LogDebug($"[LoadBundle] {Path.GetFileName(path)} -> {font.name} (#{font.instanceID})");
        }

        bundle.Unload(false);
    }

    public static void LoadFont(string path)
    {
        var fontName = Path.GetFileNameWithoutExtension(path);
        var font = TMP_FontAsset.CreateFontAsset(
            new Font(path), 100, 8, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);

        font.name = fontName;
        Fonts[fontName] = font;
        DontDestroyOnLoad(font);

        Logger.LogDebug($"[LoadFont] {font.name} (#{font.instanceID})");
    }

    public static void LoadMap(string path)
    {
        foreach (var (header, values) in Utils.LoadMapFromPath(path))
        {
            var parts = header.Split(" -> ", 2);
            if (parts.Length == 2)
            {
                var targetName = parts[0].Trim();
                var replaceName = parts[1].Trim();

                ReplaceFonts[targetName] = replaceName;
                Logger.LogDebug($"[{path}] '{targetName}' -> '{replaceName}'");
                continue;
            }

            if (!FallbackFonts.TryGetValue(header, out var fallbackFonts))
            {
                fallbackFonts = [];
                FallbackFonts[header] = fallbackFonts;
            }

            fallbackFonts.AddRange(values);
            Logger.LogDebug($"[{path}] '{header}' <- {string.Join(", ", values.Select(x => $"'{x}'"))}");
        }
    }

    internal static void ApplyFonts()
    {
        if (FallbackFonts.TryGetValue("", out var fallbackFontNames))
        {
            for (int i = fallbackFontNames.Count - 1; i >= 0; i--)
            {
                var fallbackFontName = fallbackFontNames[i];
                if (Fonts.TryGetValue(fallbackFontName, out var fallbackFont))
                {
                    TMP_Settings.fallbackFontAssets.Insert(0, fallbackFont);
                    Logger.LogDebug($"[GLOBAL] {fallbackFont.name} added as fallback");
                    continue;
                }

                Logger.LogWarning($"[GLOBAL] {fallbackFontName} not found; skipping.");
            }
        }

        foreach (var targetFont in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
        {
            if (Fonts.ContainsValue(targetFont))
            {
                continue;
            }

            if (ReplaceFonts.TryGetValue(targetFont.name, out var replaceFontName))
            {
                if (!Fonts.TryGetValue(replaceFontName, out var replaceFont))
                {
                    Logger.LogWarning($"[{targetFont.name}] {replaceFontName} not found; skipping.");
                    continue;
                }

                try
                {
                    targetFont.fallbackFontAssetTable.Clear();
                    targetFont.fallbackFontAssetTable.Add(replaceFont);

                    targetFont.characterLookupTable.Clear();

                    // Placeholders: 32=Space, 45=Hyphen, 95=Underscore
                    targetFont.characterTable.RemoveAll(c => c.unicode is not (32 or 45 or 95));
                    targetFont.characterTable.ForEach(c => targetFont.characterLookupTable[c.unicode] = c);

                    Logger.LogDebug($"[{targetFont.name}] Replaced by {replaceFont.name}");
                }
                catch (Exception e)
                {
                    Logger.LogDebug($"[{targetFont.name}] Failed to replace with {replaceFont.name}: {e.Message}");
                }
            }

            if (FallbackFonts.TryGetValue(targetFont.name, out fallbackFontNames))
            {
                for (int i = fallbackFontNames.Count - 1; i >= 0; i--)
                {
                    var fallbackFontName = fallbackFontNames[i];
                    if (Fonts.TryGetValue(fallbackFontName, out var fallbackFont))
                    {
                        targetFont.fallbackFontAssetTable.Insert(0, fallbackFont);
                        Logger.LogDebug($"[{targetFont.name}] {fallbackFont.name} added as fallback");
                        continue;
                    }

                    Logger.LogWarning($"[{targetFont.name}] {fallbackFontName} not found; skipping.");
                }
            }

            targetFont.ReadFontAssetDefinition();
        }
    }
}
