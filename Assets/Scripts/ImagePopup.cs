using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class ImagePopup : PopupBase
{
    [SerializeField] private Image image;

    public override void Show(int imageIndex)
    {
        base.Show(imageIndex);
        LoadAsync(imageIndex).Forget();
    }

    private async UniTask LoadAsync(int imageIndex)
    {
        var token = Cts.Token;

        try
        {
            var sprite = await ImageLoader.LoadSpriteAsync(imageIndex);

            if (token.IsCancellationRequested || !this || image == null)
                return;

            image.sprite = sprite;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    public override void Hide()
    {
        if (image != null)
            image.sprite = null;

        base.Hide();
    }
}