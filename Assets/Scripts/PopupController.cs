using UnityEngine;
using System;
using System.Collections.Generic;

public class PopupController : MonoBehaviour
{
    public static PopupController Instance { get; private set; }

    [SerializeField] private List<PopupBase> popups;

    private Dictionary<Type, PopupBase> _popupMap;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _popupMap = new Dictionary<Type, PopupBase>(popups.Count);

        foreach (var popup in popups)
        {
            if (popup == null)
                continue;

            _popupMap[popup.GetType()] = popup;
        }
    }

    public void Show<T>(int context) where T : PopupBase
    {
        HideAll();

        if (_popupMap.TryGetValue(typeof(T), out var popup))
            popup.Show(context);
    }

    public void HideAll()
    {
        foreach (var popup in popups)
        {
            if (popup.gameObject.activeSelf)
                popup.Hide();
        }
    }
}