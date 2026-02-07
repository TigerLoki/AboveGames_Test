using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HeaderController : MonoBehaviour,
    IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;
    [SerializeField] private HeaderIndicatorView indicator;

    [SerializeField] private float headerWidth = 2238f;
    [SerializeField] private float autoInterval = 5f;
    [SerializeField] private float animDuration = 0.4f;
    [SerializeField] private float swipeThresholdPx = 120f;

    private int _count;
    private int _index;
    
    private float _dragStartContentX;

    private CancellationTokenSource _autoCts;
    private CancellationTokenSource _animCts;

    void Start()
    {
        _count = content.childCount;
        _index = 0;

        SetInstant(_index);
        indicator?.SetInstant(_index);

        _autoCts = new CancellationTokenSource();
        AutoRotateAsync(_autoCts.Token).Forget();
    }

    void OnDestroy()
    {
        CancellationUtils.CancelAndDispose(ref _autoCts);
        CancellationUtils.CancelAndDispose(ref _animCts);
    }

    async UniTaskVoid AutoRotateAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay((int)(autoInterval * 1000), cancellationToken: token);

                GoTo((_index + 1) % _count);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    void ResetAuto()
    {
        CancellationUtils.CancelAndDispose(ref _autoCts);
        _autoCts = new CancellationTokenSource();
        AutoRotateAsync(_autoCts.Token).Forget();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStartContentX = content.anchoredPosition.x;
        CancellationUtils.CancelAndDispose(ref _animCts);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var delta = eventData.delta.x;

        if (_index == 0 && delta > 0)
            return;

        if (_index == _count - 1 && delta < 0)
            return;

        var x = content.anchoredPosition.x + delta;
        content.anchoredPosition = new Vector2(x, content.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var delta = content.anchoredPosition.x - _dragStartContentX;

        var nextIndex = _index;

        if (Mathf.Abs(delta) > swipeThresholdPx)
            nextIndex += delta < 0 ? 1 : -1;

        nextIndex = Mathf.Clamp(nextIndex, 0, _count - 1);

        GoTo(nextIndex);
        ResetAuto();
    }

    private void GoTo(int index)
    {
        if (index == _index)
        {
            AnimateTo(index, _index);
            return;
        }

        var from = _index;
        _index = index;

        AnimateTo(index, from);
    }

    private void SetInstant(int index)
    {
        content.anchoredPosition = new Vector2(GetCenteredX(index), content.anchoredPosition.y);
    }

    private void AnimateTo(int index, int from)
    {
        var targetX = GetCenteredX(index);

        CancellationUtils.CancelAndDispose(ref _animCts);
        _animCts = new CancellationTokenSource();

        AnimateAsync(targetX, from, index, _animCts.Token).Forget();
    }

    private async UniTaskVoid AnimateAsync(
        float targetX,
        int from,
        int to,
        CancellationToken token)
    {
        var startX = content.anchoredPosition.x;
        var t = 0f;

        try
        {
            while (t < 1f)
            {
                if (token.IsCancellationRequested)
                    return;

                t += Time.deltaTime / animDuration;
                var eased = EaseOutCubic(t);

                var x = Mathf.Lerp(startX, targetX, eased);
                content.anchoredPosition = new Vector2(x, content.anchoredPosition.y);

                indicator?.SetProgress(from, to, eased);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            content.anchoredPosition = new Vector2(targetX, content.anchoredPosition.y);

            indicator?.SetInstant(to);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private float GetCenteredX(int index)
    {
        var viewportWidth = viewport.rect.width;

        return -index * headerWidth + (viewportWidth - headerWidth) * 0.5f;
    }

    private static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
