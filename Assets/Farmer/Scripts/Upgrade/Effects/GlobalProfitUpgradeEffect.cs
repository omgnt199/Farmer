using Farmer;

public sealed class GlobalProfitUpgradeEffect : IUpgradeEffect
{
    public void Apply(UpgradeDefinition definition, UpgradeLevelData levelData)
    {
        MarketPriceService.AddGlobalMultiplier(levelData.effectValue);
    }
}
