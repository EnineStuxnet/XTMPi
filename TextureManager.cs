using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XTMP;

public class TextureManager
{
    internal Dictionary<string, string> Replacements = [];
    private readonly HashSet<int> _applied = [];

    public void LoadTexture(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        Replacements[name] = path;
        Plugin.Logger.LogDebug($"[LoadTexture] Registered replacement for {name} <- {Path.GetRelativePath(Utils.ConfigDir, path)}");
    }

    public void ApplyReplacements()
    {
        if (Replacements.Count == 0)
            return;

        foreach (var texture in Resources.FindObjectsOfTypeAll<Texture2D>())
        {
            if (_applied.Contains(texture.GetInstanceID()))
                continue;

            if (!Replacements.TryGetValue(texture.name, out var path))
                continue;

            var bytes = File.ReadAllBytes(path);

            if (texture.LoadImage(bytes, false))
            {
                _applied.Add(texture.GetInstanceID());
                Plugin.Logger.LogDebug($"[ApplyReplacements] {Path.GetRelativePath(Utils.ConfigDir, path)}를 {texture.name}와 물물교환했다.");
            }
            else
            {
                Plugin.Logger.LogWarning($"[ApplyReplacements] {texture.name} 이미지를 불러오는 것을 실패했다. {path}가 제대로 된 파일인지 확인해야 한다.");
            }
        }
    }
}
