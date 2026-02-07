using UnityEngine;

public class HeaderIndicatorView : MonoBehaviour
{
    [SerializeField] private RectTransform activeDot;
    [SerializeField] private RectTransform[] dotAnchors;
    [SerializeField] private float dotWidth = 25f;

    public void SetInstant(int index)
    {
        var center = dotAnchors[index].anchoredPosition.x;
        SetEdges(center - dotWidth * 0.5f, center + dotWidth * 0.5f);
    }

    public void SetProgress(int from, int to, float progress)
    {
        var fromCenter = dotAnchors[from].anchoredPosition.x;
        var toCenter = dotAnchors[to].anchoredPosition.x;

        var fromLeft = fromCenter - dotWidth * 0.5f;
        var fromRight = fromCenter + dotWidth * 0.5f;
        var toLeft = toCenter - dotWidth * 0.5f;
        var toRight = toCenter + dotWidth * 0.5f;

        var forward = to > from;

        float left;
        float right;

        if (progress < 0.5f)
        {
            var t = EaseOutCubic(progress * 2f);

            if (forward)
            {
                left = fromLeft;
                right = Mathf.Lerp(fromRight, toRight, t);
            }
            else
            {
                left = Mathf.Lerp(fromLeft, toLeft, t);
                right = fromRight;
            }
        }
        else
        {
            var t = EaseOutCubic((progress - 0.5f) * 2f);

            if (forward)
            {
                left = Mathf.Lerp(fromLeft, toLeft, t);
                right = toRight;
            }
            else
            {
                left = toLeft;
                right = Mathf.Lerp(fromRight, toRight, t);
            }
        }

        SetEdges(left, right);
    }

    private void SetEdges(float left, float right)
    {
        var containerWidth =
            ((RectTransform)activeDot.parent).rect.width;

        activeDot.offsetMin =
            new Vector2(left, activeDot.offsetMin.y);

        activeDot.offsetMax =
            new Vector2(right - containerWidth, activeDot.offsetMax.y);
    }

    private static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
