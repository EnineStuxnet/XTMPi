using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XTMP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static Dictionary<string, TMP_FontAsset> ReplaceFonts = [];
    internal static Dictionary<string, List<TMP_FontAsset>> FallbackFonts = [];

    void Awake()
    {
        Logger = base.Logger;

        var bundleDir = Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_NAME, "bundles");

        foreach (var bundlePath in Directory.EnumerateFiles(bundleDir))
        {
            if (Path.HasExtension(bundlePath))
            {
                Logger.LogDebug($"Skipping {bundlePath}");
                continue;
            }

            try
            {
                var bundle = AssetBundle.LoadFromFile(bundlePath);
                var map = Utils.LoadMapFromPath(Path.ChangeExtension(bundlePath, "ini"));

                Logger.LogInfo($"Loaded bundle '{bundle.name}' from {bundlePath}");

                foreach (var (fontName, fontValues) in map)
                {
                    var parts = fontName.Split(" -> ", 2);
                    if (parts.Length == 2)
                    {
                        var originalFontName = parts[0].Trim();
                        var replaceFontName = parts[1].Trim();
                        var replaceFont = bundle.LoadAsset<TMP_FontAsset>(replaceFontName);

                        ReplaceFonts[originalFontName] = replaceFont;
                        DontDestroyOnLoad(replaceFont);

                        Logger.LogDebug($"{bundle.name}: Replace '{originalFontName}' to '{replaceFontName}'");
                        continue;
                    }

                    if (!FallbackFonts.TryGetValue(fontName, out var fallbackList))
                    {
                        fallbackList = [];
                        FallbackFonts[fontName] = fallbackList;
                    }

                    foreach (var fallbackFontName in fontValues)
                    {
                        var fallbackFont = bundle.LoadAsset<TMP_FontAsset>(fallbackFontName);

                        fallbackList.Add(fallbackFont);
                        DontDestroyOnLoad(fallbackFont);

                        Logger.LogDebug($"{bundle.name}: Add '{fallbackFontName}' into '{fontName}' as fallback");
                    }
                }

                bundle.Unload(false);
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to load bundle from {Path.GetFileName(bundlePath)}: {e}");
            }
        }

        SceneManager.sceneLoaded += (_, _) => ApplyFonts();
    }

    internal static void ApplyFonts()
    {
        if (FallbackFonts.TryGetValue("", out var globalFallbacks))
        {
            for (var i = 0; i < globalFallbacks.Count; i++)
            {
                var fallbackFont = globalFallbacks[i];
                if (!TMP_Settings.fallbackFontAssets.Contains(fallbackFont))
                {
                    TMP_Settings.fallbackFontAssets.Insert(i, fallbackFont);
                    Logger.LogDebug($"Added '{fallbackFont.name}' to global fallback font");
                }
            }
        }

        foreach (var font in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
        {
            Logger.LogDebug($"Found '{font.name}'");

            if (ReplaceFonts.TryGetValue(font.name, out var replaceFont))
            {
                Logger.LogDebug($"Found '{font.name}' to Replace. Gutting character table...");

                font.fallbackFontAssetTable.Clear();
                font.fallbackFontAssetTable.Add(replaceFont);

                font.characterLookupTable.Clear();

                // Placeholders: 32=Space, 45=Hyphen, 95=Underscore
                font.characterTable.RemoveAll(c => c.unicode is not (32 or 45 or 95));
                font.characterTable.ForEach(c => font.characterLookupTable[c.unicode] = c);

                Logger.LogInfo($"Replaced '{font.name}' -> '{replaceFont.name}'");
            }

            if (FallbackFonts.TryGetValue(font.name, out var fallbackFonts))
            {
                for (var i = 0; i < fallbackFonts.Count; i++)
                {
                    var fallbackFont = fallbackFonts[i];
                    if (!font.fallbackFontAssetTable.Contains(fallbackFont))
                    {
                        font.fallbackFontAssetTable.Insert(i, fallbackFont);
                        Logger.LogDebug($"Added {fallbackFont.name} to fallback for {font.name}");
                    }
                }
            }

            font.ReadFontAssetDefinition();
        }
    }
}
