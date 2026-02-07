using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class StartupController : MonoBehaviour
{
    [SerializeField] private GameObject splashScreen;
    [SerializeField] private GameObject mainContent;

    void Start()
    {
        StartAsync().Forget();
    }

    private async UniTask StartAsync()
    {
        Application.targetFrameRate = 60;
        splashScreen.SetActive(true);
        mainContent.SetActive(false);

        await UniTask.Yield();

        var preloadCount = 12;
        var indices = new List<int>(preloadCount);

        for (var i = 1; i <= preloadCount; i++)
            indices.Add(i);

        try
        {
            await ImageLoader.PreloadSpritesAsync(indices);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        splashScreen.SetActive(false);
        mainContent.SetActive(true);
    }
}