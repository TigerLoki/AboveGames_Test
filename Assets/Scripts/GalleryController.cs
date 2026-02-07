using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GalleryMode
{
    All,
    Odd,
    Even
}

public class GalleryController : MonoBehaviour
{
    [SerializeField] private ModeOptionController modeOptionController;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private GameObject imageItemPrefab;

    private const int TotalImages = 66;
    private const int PremiumEvery = 4;

    private int _columnCount;
    private int _preloadCount;

    private readonly List<LazyImage> _items = new();

    void Awake()
    {
        modeOptionController.OnTabChanged += OnModeChanged;
    }

    void Start()
    {
        scrollRect.onValueChanged.AddListener(_ => UpdateVisible());
        ApplyGrid();
        RebuildGallery();
    }

    void OnDestroy()
    {
        modeOptionController.OnTabChanged -= OnModeChanged;
    }

    void OnRectTransformDimensionsChange()
    {
        ApplyGrid();
        RebuildGallery();
    }

    private void OnModeChanged(GalleryMode mode)
    {
        RebuildGallery();
    }

    private void ApplyGrid()
    {
        _columnCount = DeviceDetector.IsTablet() ? 3 : 2;

        var width = scrollRect.viewport.rect.width;
        var spacing = grid.spacing.x;
        float padding = grid.padding.left + grid.padding.right;

        var cellSize = (width - padding - spacing * (_columnCount - 1)) / _columnCount;

        grid.cellSize = new Vector2(cellSize, cellSize);

        CalculatePreload();
    }

    private void CalculatePreload()
    {
        var viewportHeight = scrollRect.viewport.rect.height;
        var rowHeight = grid.cellSize.y + grid.spacing.y;

        var preloadHeight = viewportHeight * 1.25f;
        var rows = Mathf.CeilToInt(preloadHeight / rowHeight);

        _preloadCount = rows * _columnCount;
    }

    private void RebuildGallery()
    {
        ClearGallery();
        CreateItems();
        PreloadInitial();
        ResetScroll();
    }

    private void ClearGallery()
    {
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        _items.Clear();
    }

    private void CreateItems()
    {
        var visibleIndex = 0;

        for (var fileIndex = 1; fileIndex <= TotalImages; fileIndex++)
        {
            if (!PassFilter(fileIndex))
                continue;

            visibleIndex++;
            var isPremium = visibleIndex % PremiumEvery == 0;

            var go = Instantiate(imageItemPrefab, grid.transform);
            var item = go.GetComponent<LazyImage>();

            item.Init(fileIndex, isPremium);
            _items.Add(item);
        }
    }

    private void PreloadInitial()
    {
        Canvas.ForceUpdateCanvases();

        var count = Mathf.Min(_preloadCount, _items.Count);

        for (var i = 0; i < count; i++)
            _items[i].Load();
    }

    private void ResetScroll()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void UpdateVisible()
    {
        if (_items.Count == 0)
            return;

        var viewportHeight = scrollRect.viewport.rect.height;
        var contentRect = (RectTransform)grid.transform;
        var contentY = contentRect.anchoredPosition.y;

        var cellHeight = grid.cellSize.y + grid.spacing.y;

        var firstRow = Mathf.FloorToInt(contentY / cellHeight);
        var visibleRows = Mathf.CeilToInt(viewportHeight / cellHeight) + 1;

        var startIndex = Mathf.Max(0, firstRow * _columnCount);
        var endIndex = Mathf.Min(
            _items.Count,
            (firstRow + visibleRows) * _columnCount
        );

        for (var i = startIndex; i < endIndex; i++)
            _items[i].Load();
    }

    private bool PassFilter(int index)
    {
        return modeOptionController.CurrentMode switch
        {
            GalleryMode.Odd => index % 2 == 1,
            GalleryMode.Even => index % 2 == 0,
            _ => true
        };
    }
}
