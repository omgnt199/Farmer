namespace Farmer
{
    public static class ConstructionUpgradeService
    {
        public static bool TryUpgrade(ConstructionEntity entity)
        {
            if (entity == null || !entity.TryToGetNextConstructionLevelData(out var nextLevel))
                return false;

            if (!EconomyManager.TrySpend(nextLevel.UpgradeCost))
                return false;

            return entity.SetLevel(nextLevel.Level);
        }
    }
}
