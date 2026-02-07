using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public abstract class PopupBase : MonoBehaviour
{
    [SerializeField] private Toggle closeToggle;

    protected CancellationTokenSource Cts;

    protected virtual void Awake()
    {
        if (closeToggle != null)
            closeToggle.onValueChanged.AddListener(_ => Hide()); 
    }

    public virtual void Show(int context)
    {
        CancellationUtils.CancelAndDispose(ref Cts);
        Cts = new CancellationTokenSource();
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        CancellationUtils.CancelAndDispose(ref Cts);
        gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        CancellationUtils.CancelAndDispose(ref Cts);
    }
}