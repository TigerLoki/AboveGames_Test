using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public static class ImageLoader
{
    private const string BaseUrl = "https://data.ikppbb.com/test-task-unity-data/pics/";

    private static readonly Dictionary<int, Texture2D> TextureCache = new();
    private static readonly Dictionary<int, Sprite> SpriteCache = new();

    private static readonly Dictionary<int, UniTaskCompletionSource<Sprite>> InFlight = new();

    public static async UniTask<Sprite> LoadSpriteAsync(int index)
    {
        if (SpriteCache.TryGetValue(index, out var cachedSprite))
            return cachedSprite;

        if (InFlight.TryGetValue(index, out var existing))
            return await existing.Task;

        var tcs = new UniTaskCompletionSource<Sprite>();
        InFlight[index] = tcs;

        try
        {
            var texture = await LoadTextureAsync(index);

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            SpriteCache[index] = sprite;
            tcs.TrySetResult(sprite);

            return sprite;
        }
        catch (System.Exception e)
        {
            tcs.TrySetException(e);
            throw;
        }
        finally
        {
            InFlight.Remove(index);
        }
    }

    public static async UniTask PreloadSpritesAsync(IReadOnlyList<int> indices)
    {
        var tasks = new List<UniTask>(indices.Count);

        foreach (var index in indices)
        {
            if (SpriteCache.ContainsKey(index))
                continue;

            tasks.Add(LoadSpriteAsync(index));
        }

        await UniTask.WhenAll(tasks);
    }

    private static async UniTask<Texture2D> LoadTextureAsync(int index)
    {
        if (TextureCache.TryGetValue(index, out var cached))
            return cached;

        var url = $"{BaseUrl}{index}.jpg";

        using var request = UnityWebRequestTexture.GetTexture(url);

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            throw new UnityException($"Image load failed: {url}");

        var texture = DownloadHandlerTexture.GetContent(request);
        TextureCache[index] = texture;

        return texture;
    }

    private static void ClearCache()
    {
        TextureCache.Clear();
        SpriteCache.Clear();
    }
} 
