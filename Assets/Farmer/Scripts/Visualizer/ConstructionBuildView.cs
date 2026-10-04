using System;
using DG.Tweening;
using Farmer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ConstructionBuildView : MonoBehaviour
{
    [SerializeField] private RectTransform rectContainer;
    [SerializeField] private RectTransform frame;
    [SerializeField] private Image construction_icon;
    [SerializeField] private TMP_Text construction_name;
    [SerializeField] private TMP_Text unlockPrice;
    [SerializeField] private Button unlockBtn;
    private Func<bool> onUnlockAction;
    private ConstructionDefinition constructionDefinition;

    void OnEnable()
    {
        unlockBtn.onClick.AddListener(Unlock);
        EconomyManager.OnMoneyChanged += OnMoneyChanged;
        RefreshInteractable();
    }

    void OnDisable()
    {
        unlockBtn.onClick.RemoveListener(Unlock);
        EconomyManager.OnMoneyChanged -= OnMoneyChanged;
    }

    public void Show(ConstructionContainerMono containerMono, Func<bool> onUnlock)
    {
        if (containerMono == null || containerMono.ConstructionDefinition == null)
        {
            Debug.LogError("Construction or ConstructionDefinition is missing.");
            return;
        }

        constructionDefinition = containerMono.ConstructionDefinition;
        construction_icon.sprite = constructionDefinition.icon;
        construction_name.text = constructionDefinition.displayName;
        unlockPrice.text = BigNumberFormatter.Format(constructionDefinition.unlockPrice);
        //Tween animation frame
        Vector2 localPosition = Common.WorldToCanvasPosition(GameManager.Instance.MainCamera
        , containerMono.transform.position + Vector3.up * 1f, rectContainer);
        localPosition = Common.ClampRectTransformPosition(frame, rectContainer, localPosition);
        frame.DOAnchorPos(localPosition, 0.5f).From(Vector2.zero);
        frame.DOScale(1f, 0.5f).From(0);
        onUnlockAction = onUnlock;
        gameObject.SetActive(true);
        RefreshInteractable();
    }

    public void Unlock()
    {
        if (onUnlockAction?.Invoke() == true)
            gameObject.SetActive(false);
        else
            RefreshInteractable();
    }

    private void OnMoneyChanged(CurrencyType type)
    {
        if (type == CurrencyType.Coin)
            RefreshInteractable();
    }

    private void RefreshInteractable()
    {
        if (unlockBtn != null)
            unlockBtn.interactable = constructionDefinition != null &&
            (EconomyManager.GetCurrency(CurrencyType.Coin) >= constructionDefinition.unlockPrice || EconomyManager.FreeSpend);
    }
}
