using UnityEngine;
using UnityEngine.UI;

public sealed class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Button button;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(ShowUpgradeView);
    }

    void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(ShowUpgradeView);
    }

    private void ShowUpgradeView()
    {
        if (GameUI.Instance != null)
            GameUI.Instance.ShowUpgradeView();
    }
}
