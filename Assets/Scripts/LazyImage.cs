using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

public class LazyImage : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private GameObject premiumIcon;

    private int _index;
    private bool _isPremium;
    private bool _loading;

    private CancellationTokenSource _cts;

    public void Init(int index, bool isPremium)
    {
        _index = index;
        _isPremium = isPremium;
        _loading = false;

        CancellationUtils.CancelAndDispose(ref _cts);
        _cts = new CancellationTokenSource();

        if (targetImage != null)
            targetImage.sprite = null;

        if (premiumIcon != null)
            premiumIcon.SetActive(isPremium);
    }

    public void Load()
    {
        if (_loading || targetImage == null || targetImage.sprite != null)
            return;

        _loading = true;
        LoadInternalAsync(_cts.Token).Forget();
    }

    private async UniTaskVoid LoadInternalAsync(CancellationToken token)
    {
        try
        {
            var sprite = await ImageLoader.LoadSpriteAsync(_index);

            if (token.IsCancellationRequested ||
                !this ||
                !gameObject.activeInHierarchy ||
                targetImage == null)
                return;

            targetImage.sprite = sprite;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _loading = false;
        }
    }

    public void OnClick()
    {
        if (_isPremium)
            PopupController.Instance.Show<PremiumPopup>(_index);
        else
            PopupController.Instance.Show<ImagePopup>(_index);
    }

    void OnDestroy()
    {
        CancellationUtils.CancelAndDispose(ref _cts);
    }
}