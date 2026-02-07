using UnityEngine;
using Screen = UnityEngine.Device.Screen;

public static class DeviceDetector
{
    private const float DefaultDpi = 96f;
    private const float TabletDiagonalInches = 7f;

    public static bool IsTablet()
    {
        var dpi = Screen.dpi > 0f ? Screen.dpi : DefaultDpi;

        var widthInches = Screen.width / dpi;
        var heightInches = Screen.height / dpi;

        var diagonal = Mathf.Sqrt(
            widthInches * widthInches +
            heightInches * heightInches
        );

        return diagonal >= TabletDiagonalInches;
    }
}