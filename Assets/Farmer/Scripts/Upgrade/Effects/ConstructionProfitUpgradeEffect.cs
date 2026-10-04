using Farmer;

public class ConstructionProfitUpgradeEffect : IUpgradeEffect
{
    public void Apply(UpgradeDefinition definition, UpgradeLevelData levelData)
    {
        MarketPriceService.AddConstructionMultiplier(definition.targetId, levelData.effectValue);
    }
}
