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
        public static readonly HashSet<string> Texture = [".png", ".jpg", ".jpeg"];
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

    internal static bool IsImageFile(string path)
    {
        try
        {
            using var stream = File.OpenRead(path);
            var header = new byte[8];
            var read = stream.Read(header, 0, header.Length);

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (read >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47
                && header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
                return true;

            // JPEG: FF D8 FF
            if (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return true;

            return false;
        }
        catch
        {
            return false;
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
