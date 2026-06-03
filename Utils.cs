using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using TMPro;
using UnityEngine;

namespace XTMP;

public static class Utils
{
    public static class Extensions
    {
        public static readonly HashSet<string> Bundle = ["", ".bundle", ".bundles"];
        public static readonly HashSet<string> Font = [".ttf", ".otf"];
        public static readonly HashSet<string> FontProperties = [".json", ".prop", ".props", ".properties"];
        public static readonly HashSet<string> Map = [".map", ".ini"];
    }

    public static readonly string ConfigDir = Path.Combine(Paths.PluginPath, MyPluginInfo.PLUGIN_NAME);
    public static IEnumerable<string> ConfigFiles
    {
        get => Directory.Exists(ConfigDir)
            ? Directory.EnumerateFiles(ConfigDir, "*", SearchOption.AllDirectories)
            : [];
    }

    internal static void SetupTextMeshProShader()
    {
        var shaderField = typeof(ShaderUtilities).GetField("k_ShaderRef_MobileBitmap", BindingFlags.NonPublic | BindingFlags.Static);
        if (shaderField != null)
        {
            var shader = Shader.Find("TextMeshPro/Mobile/Bitmap")
                ?? Shader.Find("TextMeshPro/Bitmap")
                ?? Shader.Find("UI/Default");

            shaderField.SetValue(null, shader);
        }
    }

    internal static Dictionary<string, List<string>> LoadMapFromPath(string path)
    {
        var map = new Dictionary<string, List<string>>() { { "", [] } };
        var key = "";

        foreach (var line in File.ReadLines(path))
        {
            var trimmedLine = line.Split(["#", ";"], 2, StringSplitOptions.None)[0].Trim();
            if (trimmedLine == "")
            {
                continue;
            }

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                key = trimmedLine.TrimStart('[').TrimEnd(']');
                map[key] = [];
                continue;
            }

            map[key].Add(trimmedLine);
        }

        return map;
    }
}