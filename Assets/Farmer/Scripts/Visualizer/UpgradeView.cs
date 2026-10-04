using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UpgradeView : MonoBehaviour
{
    [SerializeField] private List<UpgradeDefinition> definitions = new();
    [SerializeField] private UpgradeItemView itemPrefab;
    [SerializeField] private Transform itemRoot;
    [SerializeField] private Button closeButton;

    private readonly List<UpgradeItemView> items = new();

    void OnEnable()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    void OnDisable()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(Hide);
    }

    void Start()
    {
        if (itemPrefab == null || itemRoot == null)
            return;

        for (int i = 0; i < definitions.Count; i++)
        {
            if (definitions[i] == null)
                continue;

            var item = Instantiate(itemPrefab, itemRoot);
            item.Bind(definitions[i]);
            items.Add(item);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
