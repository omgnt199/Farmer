using Farmer;
using TMPro;
using UnityEngine;
public class CurrencyView : MonoBehaviour
{
    [SerializeField] private TMP_Text coinTxt;
    [SerializeField] private TMP_Text diamondTxt;

    void OnEnable()
    {
        UpdateView();
        EconomyManager.OnMoneyChanged += UpdateView;
    }
    void OnDisable()
    {
        EconomyManager.OnMoneyChanged -= UpdateView;
    }
    void UpdateView()
    {
        coinTxt.text = $"{BigNumberFormatter.Format(EconomyManager.GetCurrency(CurrencyType.Coin))}";
        diamondTxt.text = $"{BigNumberFormatter.Format(EconomyManager.GetCurrency(CurrencyType.Diamond))}";
    }
    void UpdateView(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                coinTxt.text = $"{BigNumberFormatter.Format(EconomyManager.GetCurrency(currencyType))}";
                break;
            case CurrencyType.Diamond:
                diamondTxt.text = $"{BigNumberFormatter.Format(EconomyManager.GetCurrency(currencyType))}";
                break;
        }
    }
}
