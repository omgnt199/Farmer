using Farmer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UpgradeItemView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button purchaseButton;

    private UpgradeDefinition definition;
    private Sprite defaultButtonBackground;
    private Color defaultButtonColor;

    void Awake()
    {
        if (purchaseButton == null)
            return;

        defaultButtonBackground = purchaseButton.image.sprite;
        defaultButtonColor = purchaseButton.image.color;
    }

    void OnEnable()
    {
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(Purchase);
        EconomyManager.OnMoneyChanged += OnMoneyChanged;
        UpgradeService.UpgradePurchased += OnUpgradePurchased;
    }

    void OnDisable()
    {
        if (purchaseButton != null)
            purchaseButton.onClick.RemoveListener(Purchase);
        EconomyManager.OnMoneyChanged -= OnMoneyChanged;
        UpgradeService.UpgradePurchased -= OnUpgradePurchased;
    }

    public void Bind(UpgradeDefinition value)
    {
        definition = value;
        Refresh();
    }

    private void Purchase()
    {
        UpgradeService.TryPurchase(definition);
    }

    private void Refresh()
    {
        if (definition == null)
            return;

        if (icon != null)
            icon.sprite = definition.icon;
        if (titleText != null)
            titleText.text = definition.title;

        bool hasNextLevel = UpgradeService.TryGetNextLevel(definition, out var levelData);
        gameObject.SetActive(hasNextLevel);
        if (!hasNextLevel)
            return;

        if (descriptionText != null)
            descriptionText.text = GetDescription(levelData);
        if (priceText != null)
            priceText.text = BigNumberFormatter.Format(levelData.upgradePrice);
        if (purchaseButton != null)
        {
            purchaseButton.interactable = EconomyManager.FreeSpend ||
                                           EconomyManager.GetCurrency(definition.currencyType) >= levelData.upgradePrice;
            purchaseButton.image.sprite = defaultButtonBackground;
            purchaseButton.image.color = defaultButtonColor;
        }
    }

    private string GetDescription(UpgradeLevelData levelData)
    {
        if (!string.IsNullOrWhiteSpace(definition.descriptionFormat))
            return definition.descriptionFormat.Replace("{value}", levelData.effectValue.ToString("0.##"));

        return definition.effectType switch
        {
            UpgradeEffectType.ConstructionProfit => $"x{levelData.effectValue:0.##} Profit",
            UpgradeEffectType.GlobalProfit => $"x{levelData.effectValue:0.##} Global Profit",
            UpgradeEffectType.AddCustomer => $"+{levelData.effectValue:0.##} Customer",
            _ => string.Empty
        };
    }

    private void OnMoneyChanged(CurrencyType _) => Refresh();

    private void OnUpgradePurchased(UpgradeDefinition purchasedDefinition, UpgradeState _)
    {
        if (purchasedDefinition == definition)
            Refresh();
    }
}
