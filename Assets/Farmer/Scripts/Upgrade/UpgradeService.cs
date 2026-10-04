using System;
using System.Collections.Generic;
using Farmer;

public static class UpgradeService
{
    private static readonly Dictionary<string, UpgradeState> States = new();
    private static readonly Dictionary<UpgradeEffectType, IUpgradeEffect> Effects = new()
    {
        { UpgradeEffectType.ConstructionProfit, new ConstructionProfitUpgradeEffect() },
        { UpgradeEffectType.GlobalProfit, new GlobalProfitUpgradeEffect() },
        { UpgradeEffectType.AddCustomer, new AddCustomerUpgradeEffect() }
    };

    public static event Action<UpgradeDefinition, UpgradeState> UpgradePurchased;

    public static UpgradeState GetState(UpgradeDefinition definition)
    {
        if (definition == null || string.IsNullOrWhiteSpace(definition.id))
            return null;

        if (!States.TryGetValue(definition.id, out var state))
        {
            state = new UpgradeState(definition.id);
            States.Add(definition.id, state);
        }

        return state;
    }

    public static bool TryGetNextLevel(UpgradeDefinition definition, out UpgradeLevelData levelData)
    {
        levelData = null;
        var state = GetState(definition);
        if (state == null || definition.levels == null)
            return false;

        int nextLevel = state.level + 1;
        for (int i = 0; i < definition.levels.Count; i++)
        {
            if (definition.levels[i] != null && definition.levels[i].level == nextLevel)
            {
                levelData = definition.levels[i];
                return true;
            }
        }

        return false;
    }

    public static bool TryPurchase(UpgradeDefinition definition)
    {
        if (!TryGetNextLevel(definition, out var levelData) ||
            !Effects.TryGetValue(definition.effectType, out var effect) ||
            !EconomyManager.TrySpend(levelData.upgradePrice, definition.currencyType))
            return false;

        var state = GetState(definition);
        state.level = levelData.level;
        effect.Apply(definition, levelData);
        UpgradePurchased?.Invoke(definition, state);
        return true;
    }

    public static void ResetRuntimeState()
    {
        States.Clear();
    }
}
