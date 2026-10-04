public sealed class AddCustomerUpgradeEffect : IUpgradeEffect
{
    public void Apply(UpgradeDefinition definition, UpgradeLevelData levelData)
    {
        CustomerPopulation.AddCapacity(System.Math.Max(1, (int)levelData.effectValue));
    }
}
