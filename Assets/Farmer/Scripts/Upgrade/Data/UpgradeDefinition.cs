using System.Collections.Generic;
using Farmer;
using UnityEngine;

public enum UpgradeEffectType
{
    ConstructionProfit,
    GlobalProfit,
    AddCustomer
}

[CreateAssetMenu(fileName = "_UpgradeDefinition", menuName = "Farmer/UpgradeDefine")]
public class UpgradeDefinition : ScriptableObject
{
    public string id;
    public string title;
    [Tooltip("Use {value} for the value of the next upgrade level.")]
    public string descriptionFormat;
    public Sprite icon;
    public UpgradeEffectType effectType;
    public CurrencyType currencyType = CurrencyType.Coin;
    [Tooltip("Used by effects that target a specific construction.")]
    public string targetId;
    public List<UpgradeLevelData> levels;
}
