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
    internal static Dictionary<string, List<TMP_FontAsset>> FallbackFonts;

    void Awake()
    {
        Logger = base.Logger;
        FallbackFonts = [];

        foreach (var bundlePath in Directory.GetFiles(Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_NAME, "bundles")))
        {
            if (bundlePath.Contains("."))
            {
                continue;
            }

            try
            {
                var bundle = AssetBundle.LoadFromFile(bundlePath);
                var map = Utils.LoadMapFromPath(Path.ChangeExtension(bundlePath, "ini"));

                foreach (var key in map.Keys)
                {
                    if (!FallbackFonts.ContainsKey(key))
                    {
                        FallbackFonts[key] = [];
                    }

                    foreach (var name in map[key])
                    {
                        FallbackFonts[key].Add(bundle.LoadAsset<TMP_FontAsset>(name));
                    }
                }   
            } catch (Exception e)
            {
                Logger.LogError($"Failed to load {Path.GetFileName(bundlePath)} bundle: {e}");
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (FallbackFonts.ContainsKey(""))
        {
            foreach (var fallbackFont in FallbackFonts[""])
            {
                if (!TMP_Settings.fallbackFontAssets.Contains(fallbackFont))
                {
                    TMP_Settings.fallbackFontAssets.Add(fallbackFont);
                    Logger.LogInfo($"Added {fallbackFont.name} to global fallback font");
                }
            }
        }

        foreach (var font in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
        {
            if (!FallbackFonts.ContainsKey(font.name))
            {
                continue;
            }

            foreach (var fallbackFont in FallbackFonts[font.name])
            {
                if (!font.fallbackFontAssetTable.Contains(fallbackFont))
                {
                    font.fallbackFontAssetTable.Add(fallbackFont);
                    Logger.LogInfo($"Added {fallbackFont.name} to fallback for {font.name}");
                }
            }
        }
    }
}
