using System;
using System.Collections.Generic;
namespace Farmer
{
    public class ConstructionEntity
    {
        private int level;
        private int maxLevel;
        private readonly ConstructionDefinition definition;
        private readonly Dictionary<int, ConstructionLevelData> levelDataByLevel = new();

        public ConstructionDefinition Definition => definition;
        public event Action<ConstructionEntity> LevelChanged;

        public ConstructionEntity(ConstructionDefinition definition)
        {
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            var levels = definition.constructionUpgradePriceDefine?.upgradeLevelDatas;

            if (levels == null || levels.Count == 0)
                throw new InvalidOperationException($"{definition.name} has no construction level data.");

            int minLevel = int.MaxValue;
            foreach (var levelData in levels)
            {
                if (levelData == null || levelData.Level < 1)
                    continue;

                levelDataByLevel[levelData.Level] = levelData;
                minLevel = Math.Min(minLevel, levelData.Level);
                maxLevel = Math.Max(maxLevel, levelData.Level);
            }

            if (levelDataByLevel.Count == 0)
                throw new InvalidOperationException($"{definition.name} has no valid construction level data.");

            level = minLevel;
        }

        public int GetLevel() => level;
        public int GetMaxLevel() => maxLevel;
        public bool IsMaxLevel() => level >= maxLevel;

        public bool TryToGetCurrentConstructionLevelData(out ConstructionLevelData result)
        {
            return levelDataByLevel.TryGetValue(level, out result);
        }

        public bool TryToGetNextConstructionLevelData(out ConstructionLevelData result)
        {
            return levelDataByLevel.TryGetValue(level + 1, out result);
        }

        public BigNumber GetProfit()
        {
            return TryToGetCurrentConstructionLevelData(out var data)
                ? MarketPriceService.CalculateSellPrice(definition.id, data.HarvestValue)
                : BigNumber.Zero;
        }

        public BigNumber GetProfitPerMinute()
        {
            float harvestDuration = GetHarvestDuration();
            return harvestDuration > 0f
                ? GetProfit() * (60d / harvestDuration)
                : BigNumber.Zero;
        }

        public HarvestItem Harvest()
        {
            return new HarvestItem(definition.id, GetProfit(), definition.productPrefab);
        }

        public float GetHarvestDuration()
        {
            return TryToGetCurrentConstructionLevelData(out var data)
                ? Math.Max(0f, data.HarvestDurationPerCycle)
                : 0f;
        }

        public bool SetLevel(int value)
        {
            if (!levelDataByLevel.ContainsKey(value) || value == level)
                return false;

            level = value;
            LevelChanged?.Invoke(this);
            return true;
        }
    }
}
