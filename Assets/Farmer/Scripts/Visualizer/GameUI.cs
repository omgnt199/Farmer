using System;
using DG.Tweening;
using Farmer;
using UnityEngine;
public class GameUI : Singleton<GameUI>
{
    [SerializeField] private RectTransform mainRect;
    [SerializeField] private ConstructionBuildView constructionBuildView;
    [SerializeField] private ConstructionUpgradeLevelView constructionUpgradeLevelView;
    [SerializeField] private ConstructionUnlockCounterView constructionUnlockCounterViewPrefab;
    [SerializeField] private GameObject constructionInformationPrefab;
    [SerializeField] private UpgradeView upgradeView;

    public ConstructionBuildView ConstructionBuildView
    {
        get
        {
            if (constructionBuildView == null)
            {
                Debug.LogError("ConstructionBuildView not assigned");
            }
            return constructionBuildView;
        }
    }

    public void ShowConstructionUnlockCounterView(ConstructionContainerMono containerMono, Action onCounterEnd)
    {
        var counterViewPrefab = Instantiate(constructionUnlockCounterViewPrefab, mainRect);
        var counterRect = counterViewPrefab.GetComponent<RectTransform>();
        Vector2 localPosition = Common.WorldToCanvasPosition(GameManager.Instance.MainCamera
            , containerMono.transform.position + Vector3.up * 1f, mainRect);
        localPosition = Common.ClampRectTransformPosition(counterRect, mainRect, localPosition);
        counterRect.DOAnchorPos(localPosition, 0.5f).From(Vector2.zero);
        counterRect.DOScale(1f, 0.5f).From(0);
        counterViewPrefab.Show(containerMono.ConstructionDefinition.unlockDuration, onCounterEnd);
    }
    public void ShowConstructionBuildView(ConstructionContainerMono containerMono, Func<bool> onUnlock)
    {
        constructionBuildView.Show(containerMono, onUnlock);
    }

    public void ShowConstructionUpgradeLevelView(ConstructionMono containerMono, Action onUpgrade)
    {
        constructionUpgradeLevelView.Show(containerMono, onUpgrade);
    }

    public void ShowConstructionInformation(ConstructionMono constructionMono)
    {
        if (constructionInformationPrefab == null || mainRect == null || GameManager.Instance == null)
            return;

        var information = Instantiate(constructionInformationPrefab, mainRect);
        if (information.TryGetComponent<ConstructionInformationView>(out var view))
            view.Bind(constructionMono, mainRect, GameManager.Instance.MainCamera);
        else
            Destroy(information);
    }

    public void ShowUpgradeView()
    {
        if (upgradeView != null)
            upgradeView.Show();
    }
}
