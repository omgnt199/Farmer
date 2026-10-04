public interface IUpgradeEffect
{
    void Apply(UpgradeDefinition definition, UpgradeLevelData levelData);
}