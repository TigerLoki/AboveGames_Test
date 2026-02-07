using UnityEngine;
using UnityEngine.UI;

public class PremiumPopup : PopupBase
{
    [SerializeField] private ScrollRect scrollRect;

    public override void Show(int context)
    {
        base.Show(context);
        ResetScroll();
    }

    private void ResetScroll()
    {
        if (scrollRect == null)
            return;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}