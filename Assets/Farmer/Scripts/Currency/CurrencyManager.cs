using System;

namespace Farmer
{
    public enum CurrencyType
    {
        Coin,
        Diamond
    }
    public class CurrencyData
    {
        public BigNumber Coin;
        public BigNumber Diamond;
        public CurrencyData()
        {

        }
        public CurrencyData(BigNumber coin, BigNumber diamond)
        {
            this.Coin = coin;
            this.Diamond = diamond;
        }
    }
    public static class EconomyManager
    {
        private static CurrencyData currencyData;

        public static event Action<CurrencyType> OnMoneyChanged;
        public static bool FreeSpend { get; set; }

        public static BigNumber GetCurrency(CurrencyType type)
        {
            EnsureInitialized();

            switch (type)
            {
                case CurrencyType.Coin:
                    return currencyData.Coin;

                case CurrencyType.Diamond:
                    return currencyData.Diamond;
            }
            return BigNumber.Zero;
        }

        public static void AddMoney(BigNumber amount, CurrencyType type = CurrencyType.Coin)
        {
            EnsureInitialized();

            switch (type)
            {
                case CurrencyType.Coin:
                    currencyData.Coin += amount;
                    break;
                case CurrencyType.Diamond:
                    currencyData.Diamond += amount;
                    break;
            }

            OnMoneyChanged?.Invoke(type);
        }

        public static bool TrySpend(BigNumber amount, CurrencyType type = CurrencyType.Coin)
        {
            if (amount < BigNumber.Zero)
                return false;

            if (FreeSpend)
                return true;

            if (GetCurrency(type) < amount)
                return false;

            AddMoney(new BigNumber(-amount.Mantissa, amount.Exponent), type);
            return true;
        }

        public static void Initialize(BigNumber coin, BigNumber diamond)
        {
            currencyData = new CurrencyData(coin, diamond);
            OnMoneyChanged?.Invoke(CurrencyType.Coin);
            OnMoneyChanged?.Invoke(CurrencyType.Diamond);
        }

        private static void EnsureInitialized()
        {
            if (currencyData == null)
                currencyData = new CurrencyData(new BigNumber(1000), BigNumber.Zero);
        }
    }

    // Kept as a thin compatibility layer for existing scenes and third-party scripts.
    public static class CurrencyManager
    {
        public static event Action<CurrencyType> onCurrencyUpdate
        {
            add => EconomyManager.OnMoneyChanged += value;
            remove => EconomyManager.OnMoneyChanged -= value;
        }

        public static BigNumber GetCurrency(CurrencyType type) => EconomyManager.GetCurrency(type);
        public static void UpdateCurrency(CurrencyType type, BigNumber value) => EconomyManager.AddMoney(value, type);
    }
}
