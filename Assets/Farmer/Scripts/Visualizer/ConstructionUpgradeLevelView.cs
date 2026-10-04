using System;
using DG.Tweening;
using Farmer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConstructionUpgradeLevelView : MonoBehaviour
{
    [SerializeField] private RectTransform rectContainer;
    [SerializeField] private RectTransform frame;
    [SerializeField] private Slider levelProgressBar;
    [SerializeField] private TMP_Text levelTxt;
    [SerializeField] private TMP_Text constructionNameTxt;
    [SerializeField] private TMP_Text durationTxt;
    [SerializeField] private TMP_Text upgradePriceTxt;
    [SerializeField] private TMP_Text profitPriceTxt;
    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Image upgradeBtnBg;
    [SerializeField] private Color32 maxLevelBtnBgColor;
    [SerializeField] private GameObject maxLevelBtn;
    private Action onUpgrade;
    private ConstructionEntity activeEntity;

    void OnEnable()
    {
        upgradeBtn.onClick.AddListener(Upgrade);
        EconomyManager.OnMoneyChanged += OnMoneyChanged;
    }

    void OnDisable()
    {
        upgradeBtn.onClick.RemoveListener(Upgrade);
        EconomyManager.OnMoneyChanged -= OnMoneyChanged;
        if (activeEntity != null)
            activeEntity.LevelChanged -= UpdateView;
        activeEntity = null;
    }

    public void Show(ConstructionMono constructionMono, Action onUpgrade)
    {
        if (constructionMono == null || constructionMono.ConstructionEntity == null)
            return;

        if (activeEntity != null)
            activeEntity.LevelChanged -= UpdateView;

        activeEntity = constructionMono.ConstructionEntity;
        activeEntity.LevelChanged += UpdateView;
        this.onUpgrade = onUpgrade;
        UpdateView(activeEntity);

        //Tween animation frame
        Vector2 localPosition = Common.WorldToCanvasPosition(GameManager.Instance.MainCamera
        , constructionMono.transform.position + Vector3.up * 1f, rectContainer);
        localPosition = Common.ClampRectTransformPosition(frame, rectContainer, localPosition);
        frame.DOAnchorPos(localPosition, 0.5f).From(Vector2.zero);
        frame.DOScale(1f, 0.5f).From(0);

        gameObject.SetActive(true);
    }

    public void UpdateView(ConstructionEntity constructionEntity)
    {
        if (constructionEntity.TryToGetCurrentConstructionLevelData(out var currentLevelData))
        {
            levelTxt.text = $"Level{constructionEntity.GetLevel()}";
            durationTxt.text = $"{currentLevelData.Duration}s";
            profitPriceTxt.text = $"{BigNumberFormatter.Format(constructionEntity.GetProfitPerMinute())}/min";
            if (levelProgressBar != null)
                levelProgressBar.value = (float)constructionEntity.GetLevel() / constructionEntity.GetMaxLevel();
        }
        constructionNameTxt.text = $"{constructionEntity.Definition.displayName}";
        bool hasNextLevel = constructionEntity.TryToGetNextConstructionLevelData(out var nextLevelData);
        upgradePriceTxt.text = hasNextLevel
            ? BigNumberFormatter.Format(nextLevelData.UpgradeCost)
            : string.Empty;
        upgradePriceTxt.gameObject.SetActive(hasNextLevel);

        bool isMaxLevel = constructionEntity.IsMaxLevel();
        maxLevelBtn.SetActive(isMaxLevel);
        upgradeBtn.interactable = hasNextLevel &&
                                 (EconomyManager.GetCurrency(CurrencyType.Coin) >= nextLevelData.UpgradeCost
                                 || EconomyManager.FreeSpend);
        upgradeBtnBg.color = isMaxLevel ? maxLevelBtnBgColor : Color.white;
    }

    private void Upgrade()
    {
        onUpgrade?.Invoke();
    }

    private void OnMoneyChanged(CurrencyType currencyType)
    {
        if (currencyType == CurrencyType.Coin && activeEntity != null)
            UpdateView(activeEntity);
    }
}
