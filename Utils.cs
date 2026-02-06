using System.Collections.Generic;
using System.IO;

namespace XTMP;

public static class Utils
{
    internal static Dictionary<string, List<string>> LoadMapFromPath(string path)
    {
        var map = new Dictionary<string, List<string>>();
        var key = "";

        foreach (var line in File.ReadLines(path))
        {
            var trimmedLine = line.Trim();
            if (trimmedLine == "" || trimmedLine.StartsWith("#") || trimmedLine.StartsWith(";"))
            {
                continue;
            }

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                key = trimmedLine.Trim('[', ']');
                continue;
            }

            if (!map.ContainsKey(key))
            {
                map[key] = [];
            }

            map[key].Add(line);
        }

        return map;
    }
}