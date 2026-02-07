using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ToggleGroupController : MonoBehaviour
{
    [System.Serializable]
    protected struct ToggleItem
    {
        public Toggle toggle;
        public TMP_Text[] texts;
    }

    [SerializeField] protected ToggleItem[] items;

    [SerializeField] protected Color normalColor = Color.black;
    [SerializeField] protected Color activeColor = new Color32(255, 26, 151, 255);

    protected virtual void Awake()
    {
        for (var i = 0; i < items.Length; i++)
        {
            var index = i;
            items[i].toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                    OnItemSelected(index);
            });
        }
    }

    protected virtual void Start()
    {
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i].toggle.isOn)
            {
                Select(i, true);
                return;
            }
        }

        items[0].toggle.SetIsOnWithoutNotify(true);
        Select(0, true);
    }

    protected void Select(int index, bool force = false)
    {
        UpdateColors();
        OnSelectionChanged(index, force);
    }

    protected void UpdateColors()
    {
        foreach (var item in items)
        {
            var active = item.toggle.isOn;

            foreach (var text in item.texts)
                text.color = active ? activeColor : normalColor;
        }
    }

    protected virtual void OnItemSelected(int index)
    {
        Select(index);
    }

    protected abstract void OnSelectionChanged(int index, bool force);
}