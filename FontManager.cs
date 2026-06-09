using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace XTMP;

public class FontManager
{
    internal Dictionary<string, object> Caches = [];
    internal Dictionary<string, string> Replacements = [];
    internal Dictionary<string, List<string>> Fallbacks = [];

    public bool TryGet(string name, out TMP_FontAsset font, TMP_FontAsset baseFont = null)
    {
        font = null;

        if (!Caches.TryGetValue(name, out var value))
            return false;

        switch (value)
        {
            case string path:
                font = TMP_FontAsset.CreateFontAsset(
                    new Font(path),
                    baseFont?.faceInfo.pointSize ?? 100,
                    baseFont?.atlasPadding ?? 10,
                    baseFont?.atlasRenderMode ?? GlyphRenderMode.SDFAA,
                    2048, 2048,
                    TMPro.AtlasPopulationMode.Dynamic, true
                );

                if (baseFont != null)
                {
                    font.material = TMP_MaterialManager.GetFallbackMaterial(baseFont.material, font.material);
                    font.material.shaderKeywords = baseFont.material.shaderKeywords;
                }

                font.name = name;

                Caches[name] = font;

                ApplyCustomProperties(font);

                Plugin.DontDestroyOnLoad(font);
                Plugin.Logger.LogDebug($"[GetReferencedFontAsset] Created {font} from {Path.GetRelativePath(Utils.ConfigDir, path)}");
                return true;

            case TMP_FontAsset cachedFont:
                font = cachedFont;
                return true;
        }

        return false;
    }

    public bool TryGetReplacement(TMP_FontAsset baseFont, out TMP_FontAsset replacementFont)
    {
        replacementFont = null;

        if (Replacements.TryGetValue(baseFont?.name ?? "", out var replacementName))
            return TryGet(replacementName, out replacementFont, baseFont);

        return false;
    }

    public bool ApplyCustomProperties(TMP_FontAsset font)
    {
        var path = Directory.EnumerateFiles(Utils.ConfigDir, "*", SearchOption.AllDirectories)
            .Where(p =>
                Path.GetFileNameWithoutExtension(p) == font.name &&
                Utils.Extensions.FontProperties.Contains(Path.GetExtension(p).ToLower())
            )
            .FirstOrDefault();

        if (path == null)
            return false;

        // TODO: cleanup this mess lol
        var faceInfo = font.faceInfo;
        foreach (var property in JObject.Parse(File.ReadAllText(path)).Properties())
        {
            var value = property.Value.Value<float>();
            switch (property.Name)
            {
                case "scale": faceInfo.scale = value; break;
                case "lineHeight": faceInfo.lineHeight = value; break;
                case "ascentLine": faceInfo.ascentLine = value; break;
                case "capLine": faceInfo.capLine = value; break;
                case "meanLine": faceInfo.meanLine = value; break;
                case "baseline": faceInfo.baseline = value; break;
                case "descentLine": faceInfo.descentLine = value; break;
                case "underlineOffset": faceInfo.underlineOffset = value; break;
                case "underlineThickness": faceInfo.underlineThickness = value; break;
                case "strikethroughOffset": faceInfo.strikethroughOffset = value; break;
                case "strikethroughThickness": faceInfo.strikethroughThickness = value; break;
                case "superscriptOffset": faceInfo.superscriptOffset = value; break;
                case "superscriptSize": faceInfo.superscriptSize = value; break;
                case "subscriptOffset": faceInfo.subscriptOffset = value; break;
                case "subscriptSize": faceInfo.subscriptSize = value; break;
            }
        }
        font.faceInfo = faceInfo;

        Plugin.Logger.LogDebug($"[ApplyCustomProperties] Apply {Path.GetRelativePath(Utils.ConfigDir, path)} to {font}");
        return true;
    }

    public void LoadBundle(string path)
    {
        var bundle = AssetBundle.LoadFromFile(path);

        foreach (var font in bundle.LoadAllAssets<TMP_FontAsset>())
        {
            ApplyCustomProperties(font);
            Caches[font.name] = font;

            Plugin.Logger.LogDebug($"[LoadBundle] {Path.GetRelativePath(Utils.ConfigDir, path)} contains {font}");
        }
    }

    public void LoadFont(string path)
    {
        var fontName = Path.GetFileNameWithoutExtension(path);
        Caches[fontName] = path;
        Plugin.Logger.LogDebug($"[LoadFont] {Path.GetRelativePath(Utils.ConfigDir, path)}");
    }

    public void LoadMap(string path)
    {
        var prefix = $"[Map:{Path.GetRelativePath(Utils.ConfigDir, path)}]";

        foreach (var (header, values) in Utils.LoadMapFromPath(path))
        {
            var parts = header.Split(" -> ", 2);
            var targetName = parts[0].Trim();

            if (parts.Length == 2)
            {
                var replaceName = parts[1].Trim();
                Replacements[targetName] = replaceName;
                Plugin.Logger.LogDebug($"{prefix} {targetName} -> {replaceName}");
            }

            if (!Fallbacks.TryGetValue(targetName, out var fallbacks))
            {
                fallbacks = [];
                Fallbacks[targetName] = fallbacks;
            }

            fallbacks.AddRange(values);
            Plugin.Logger.LogDebug($"{prefix} {targetName} <- {string.Join(", ", values)}");
        }
    }
}
