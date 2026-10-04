using System.Collections.Generic;
using Farmer;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class FarmerArchitectureTests
{
    [SetUp]
    public void SetUp()
    {
        EconomyManager.FreeSpend = false;
        EconomyManager.Initialize(BigNumber.Zero, BigNumber.Zero);
        MarketPriceService.ResetRuntimeModifiers();
        MarketInventory.Clear();
        UpgradeService.ResetRuntimeState();
        CharacterTaskSystem.Clear();
        CustomerPopulation.Reset();
    }

    [Test]
    public void HarvestPrice_IsSnapshottedBeforeLaterBuffs()
    {
        var item = new HarvestItem(
            "construction-1",
            MarketPriceService.CalculateSellPrice("construction-1", new BigNumber(100)));

        MarketPriceService.AddGlobalMultiplier(2d);

        Assert.That(item.SellPrice, Is.EqualTo(new BigNumber(100)));
        Assert.That(
            MarketPriceService.CalculateSellPrice("construction-1", new BigNumber(100)),
            Is.EqualTo(new BigNumber(200)));
    }

    [Test]
    public void MarketInventory_PaysOnlyWhenCustomerBuys()
    {
        MarketInventory.Store(new HarvestItem("construction-1", new BigNumber(125)));

        Assert.That(EconomyManager.GetCurrency(CurrencyType.Coin), Is.EqualTo(BigNumber.Zero));
        Assert.That(MarketInventory.TryTake(out var item), Is.True);

        MarketPriceService.Sell(item);

        Assert.That(EconomyManager.GetCurrency(CurrencyType.Coin), Is.EqualTo(new BigNumber(125)));
    }

    [Test]
    public void StartingBalance_CanUnlockFirstConstruction()
    {
        var definition = AssetDatabase.LoadAssetAtPath<ConstructionDefinition>(
            "Assets/Farmer/ScriptableObjects/Construction_1_ConstructionDefinition.asset");
        EconomyManager.Initialize(new BigNumber(1000), BigNumber.Zero);

        Assert.That(definition, Is.Not.Null);
        Assert.That(EconomyManager.TrySpend(definition.unlockPrice), Is.True);
    }

    [Test]
    public void FreeSpend_AllowsAnyCurrencyWithoutChangingItsBalance()
    {
        EconomyManager.FreeSpend = true;

        Assert.That(EconomyManager.TrySpend(new BigNumber(999), CurrencyType.Diamond), Is.True);
        Assert.That(EconomyManager.GetCurrency(CurrencyType.Diamond), Is.EqualTo(BigNumber.Zero));
    }

    [Test]
    public void ConstructionUpgrade_SpendsMoneyAndAdvancesOneLevel()
    {
        var levels = ScriptableObject.CreateInstance<ConstructionUpgradePriceDefine>();
        var definition = ScriptableObject.CreateInstance<ConstructionDefinition>();

        try
        {
            levels.upgradeLevelDatas = new List<ConstructionLevelData>
            {
                new()
                {
                    Level = 1,
                    UpgradeCost = new BigNumber(10),
                    HarvestValue = new BigNumber(100),
                    HarvestDurationPerCycle = 10f
                },
                new() { Level = 2, UpgradeCost = new BigNumber(500), HarvestValue = new BigNumber(150) }
            };
            definition.id = "construction-test";
            definition.constructionUpgradePriceDefine = levels;
            EconomyManager.Initialize(new BigNumber(1000), BigNumber.Zero);

            var entity = new ConstructionEntity(definition);
            Assert.That(entity.GetProfitPerMinute(), Is.EqualTo(new BigNumber(600)));

            bool upgraded = ConstructionUpgradeService.TryUpgrade(entity);

            Assert.That(upgraded, Is.True);
            Assert.That(entity.GetLevel(), Is.EqualTo(2));
            Assert.That(EconomyManager.GetCurrency(CurrencyType.Coin), Is.EqualTo(new BigNumber(500)));
        }
        finally
        {
            Object.DestroyImmediate(definition);
            Object.DestroyImmediate(levels);
        }
    }

    [Test]
    public void DataDrivenUpgrade_AppliesConfiguredGlobalProfitEffect()
    {
        var definition = ScriptableObject.CreateInstance<UpgradeDefinition>();

        try
        {
            definition.id = "global-profit-test";
            definition.effectType = UpgradeEffectType.GlobalProfit;
            definition.levels = new List<UpgradeLevelData>
            {
                new()
                {
                    level = 1,
                    upgradePrice = new BigNumber(10),
                    effectValue = 2f
                }
            };
            EconomyManager.Initialize(new BigNumber(100), BigNumber.Zero);

            bool purchased = UpgradeService.TryPurchase(definition);

            Assert.That(purchased, Is.True);
            Assert.That(EconomyManager.GetCurrency(CurrencyType.Coin), Is.EqualTo(new BigNumber(90)));
            Assert.That(
                MarketPriceService.CalculateSellPrice("any-construction", new BigNumber(100)),
                Is.EqualTo(new BigNumber(200)));
        }
        finally
        {
            Object.DestroyImmediate(definition);
        }
    }
}
