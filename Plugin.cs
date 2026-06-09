using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XTMP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static FontManager Manager = new();

    [HarmonyPatch(typeof(TextMeshProUGUI), "LoadFontAsset")]
    [HarmonyPatch(typeof(TextMeshPro), "LoadFontAsset")]
    [HarmonyPrefix]
    public static void LoadFontAssetPrefix(TMP_Text __instance, ref TMP_FontAsset ___m_fontAsset)
    {
        if (Manager.TryGetReplacement(___m_fontAsset, out var newFont))
        {
            Logger.LogDebug($"[LoadFontAssetPrefix:{__instance}] Replace {___m_fontAsset} to {newFont}");
            ___m_fontAsset = newFont;
        }
    }

    void Awake()
    {
        Logger = base.Logger;

        Utils.SetupTextMeshProShader();

        var loaders = new Dictionary<string, Action<string>>();
        foreach (var ext in Utils.Extensions.Font) loaders[ext.ToLower()] = Manager.LoadFont;
        foreach (var ext in Utils.Extensions.Bundle) loaders[ext.ToLower()] = Manager.LoadBundle;
        foreach (var ext in Utils.Extensions.Map) loaders[ext.ToLower()] = Manager.LoadMap;

        foreach (var path in Utils.ConfigFiles)
        {
            try
            {
                var extension = Path.GetExtension(path).ToLower();
                if (!loaders.TryGetValue(extension, out var loader))
                {
                    Logger.LogDebug($"No loader, skipping {Path.GetRelativePath(Utils.ConfigDir, path)}");
                    continue;
                }

                loader(path);
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to load {Path.GetRelativePath(Utils.ConfigDir, path)}: {e}");
            }
        }

        if (Manager.Fallbacks.TryGetValue("", out var fontNames))
        {
            for (int i = fontNames.Count - 1; i >= 0; i--)
            {
                if (Manager.TryGet(fontNames[i], out var fallbackFont, TMP_Settings.defaultFontAsset))
                {
                    TMP_Settings.fallbackFontAssets.Insert(0, fallbackFont);
                    Logger.LogDebug($"[GLOBAL] added {fallbackFont} as fallback");
                }
            }
        }

        SceneManager.sceneLoaded += (scene, sceneMode) =>
        {
            foreach (var font in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if (!Manager.Caches.ContainsKey(font.name))
                {
                    var props = new Dictionary<string, object>()
                    {
                        ["name"] = font.name,
                        ["scale"] = font.faceInfo.scale,
                        ["pointSize"] = font.faceInfo.pointSize,
                        ["atlasPadding"] = font.atlasPadding,
                        ["atlasRenderMode"] = font.atlasRenderMode
                    };

                    Logger.LogDebug($"Scene {scene.name} contains {font}:\n{string.Join("\n", props.Select(x => $"\t{x.Key} = {x.Value}"))}");
                }

                if (Manager.Fallbacks.TryGetValue(font.name, out fontNames))
                {
                    for (int i = fontNames.Count - 1; i >= 0; i--)
                    {
                        if (Manager.TryGet(fontNames[i], out var fallbackFont, font))
                        {
                            if (font.fallbackFontAssetTable.Contains(fallbackFont))
                            {
                                continue;
                            }

                            font.fallbackFontAssetTable.Insert(0, fallbackFont);
                            Logger.LogDebug($"[{font}] added {fallbackFont} as fallback");
                        }
                    }
                }

                font.ReadFontAssetDefinition();
            }
        };

        Harmony.CreateAndPatchAll(typeof(Plugin), MyPluginInfo.PLUGIN_GUID);
    }
}