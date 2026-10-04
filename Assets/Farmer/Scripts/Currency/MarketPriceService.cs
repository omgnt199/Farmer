using System;
using System.Collections.Generic;
using UnityEngine;

namespace Farmer
{
    [Serializable]
    public readonly struct HarvestItem
    {
        public string ProductId { get; }
        public BigNumber SellPrice { get; }
        public GameObject ProductPrefab { get; }

        public HarvestItem(string productId, BigNumber sellPrice, GameObject productPrefab = null)
        {
            ProductId = productId;
            SellPrice = sellPrice;
            ProductPrefab = productPrefab;
        }
    }

    public static class MarketPriceService
    {
        private static readonly Dictionary<string, double> ConstructionMultipliers = new();
        private static double globalMultiplier = 1d;

        public static BigNumber CalculateSellPrice(string constructionId, BigNumber basePrice)
        {
            double constructionMultiplier = ConstructionMultipliers.TryGetValue(constructionId, out var value)
                ? value
                : 1d;
            return basePrice * constructionMultiplier * globalMultiplier;
        }

        public static void AddConstructionMultiplier(string constructionId, double multiplier)
        {
            if (string.IsNullOrWhiteSpace(constructionId) || multiplier <= 0d)
                return;

            ConstructionMultipliers[constructionId] = ConstructionMultipliers.TryGetValue(constructionId, out var current)
                ? current * multiplier
                : multiplier;
        }

        public static void AddGlobalMultiplier(double multiplier)
        {
            if (multiplier > 0d)
                globalMultiplier *= multiplier;
        }

        public static void Sell(HarvestItem item)
        {
            EconomyManager.AddMoney(item.SellPrice);
        }

        public static void ResetRuntimeModifiers()
        {
            ConstructionMultipliers.Clear();
            globalMultiplier = 1d;
        }
    }
}
