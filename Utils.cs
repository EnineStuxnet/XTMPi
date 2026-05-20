using System;
using System.Collections.Generic;
using System.IO;

namespace XTMP;

public static class Utils
{
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

            map[key].Add(line);
        }

        return map;
    }
}