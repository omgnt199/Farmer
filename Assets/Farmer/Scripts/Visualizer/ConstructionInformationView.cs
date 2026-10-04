using Farmer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ConstructionInformationView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text profitText;
    [SerializeField] private TMP_Text profitPerMinuteText;
    [SerializeField] private float verticalOffset = 0.5f;

    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Camera worldCamera;
    private Transform target;
    private ConstructionEntity entity;
    private float targetHeight;

    public void Bind(ConstructionMono construction, RectTransform parent, Camera camera)
    {
        if (construction == null || construction.ConstructionEntity == null || parent == null || camera == null)
        {
            Destroy(gameObject);
            return;
        }

        rectTransform = (RectTransform)transform;
        canvasRect = parent;
        worldCamera = camera;
        target = construction.transform;
        entity = construction.ConstructionEntity;
        targetHeight = GetTargetHeight(construction);
        entity.LevelChanged += OnLevelChanged;
        UpgradeService.UpgradePurchased += OnUpgradePurchased;
        Refresh();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (canvasRect == null || worldCamera == null)
            return;

        Vector2 position = Common.WorldToCanvasPosition(
            worldCamera,
            target.position + Vector3.up * targetHeight,
            canvasRect);
        rectTransform.anchoredPosition = Common.ClampRectTransformPosition(rectTransform, canvasRect, position);
    }

    void OnDestroy()
    {
        if (entity != null)
            entity.LevelChanged -= OnLevelChanged;
        UpgradeService.UpgradePurchased -= OnUpgradePurchased;
    }

    private void OnLevelChanged(ConstructionEntity _) => Refresh();
    private void OnUpgradePurchased(UpgradeDefinition _, UpgradeState __) => Refresh();

    private void Refresh()
    {
        if (entity == null)
            return;

        if (icon != null)
            icon.sprite = entity.Definition.icon;
        if (profitText != null)
            profitText.text = BigNumberFormatter.Format(entity.GetProfit());
        if (profitPerMinuteText != null)
            profitPerMinuteText.text = $"{BigNumberFormatter.Format(entity.GetProfitPerMinute())}/min";
    }

    private float GetTargetHeight(ConstructionMono construction)
    {
        float height = verticalOffset;
        var renderers = construction.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
            height = Mathf.Max(height, renderers[i].bounds.max.y - construction.transform.position.y + verticalOffset);

        return height;
    }
}
