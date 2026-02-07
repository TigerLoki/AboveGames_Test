public class ModeOptionController : ToggleGroupController
{
    public GalleryMode CurrentMode { get; private set; }

    public event System.Action<GalleryMode> OnTabChanged;

    protected override void OnSelectionChanged(int index, bool force)
    {
        var mode = (GalleryMode)index;

        if (!force && CurrentMode == mode)
            return;

        CurrentMode = mode;
        OnTabChanged?.Invoke(mode);
    }
}