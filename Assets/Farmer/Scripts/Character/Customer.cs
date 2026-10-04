using Farmer;
using UnityEngine;

public class Customer : Character
{
    public void MoveToQueue(Transform queuePoint, System.Action<Customer> onArrived = null)
    {
        EnqueueTask(new CharacterTask(
            CharacterTaskType.MoveToQueue,
            queuePoint,
            0f,
            CharacterState.Waiting,
            _ => onArrived?.Invoke(this)));
    }

    public void Buy(HarvestItem item, float buyDuration = 0.5f)
    {
        Buy(item, buyDuration, null);
    }

    public void ReceiveProduct(
        HarvestItem item,
        float buyDuration,
        Vector3 productStartPosition,
        System.Action<HarvestItem> onPaid,
        System.Action onProductLanded)
    {
        SetCarryingProduct(item.ProductPrefab, productStartPosition, () =>
        {
            Buy(item, buyDuration, onPaid);
            onProductLanded?.Invoke();
        });
    }

    private void Buy(HarvestItem item, float buyDuration, System.Action<HarvestItem> onPaid)
    {
        EnqueueTask(new CharacterTask(
            CharacterTaskType.Buy,
            null,
            buyDuration,
            CharacterState.Buying,
            _ =>
            {
                if (onPaid != null)
                    onPaid(item);
                else
                    MarketPriceService.Sell(item);
            }));
    }

    public bool TryBuyNext(float buyDuration = 0.5f)
    {
        if (!MarketInventory.TryTake(out var item))
            return false;

        SetCarryingProduct(item.ProductPrefab, transform.position, () => Buy(item, buyDuration));
        return true;
    }

    public void Leave(Transform exitPoint, System.Action<Customer> onLeft = null)
    {
        EnqueueTask(new CharacterTask(
            CharacterTaskType.Leave,
            exitPoint,
            0f,
            CharacterState.Leaving,
            _ =>
            {
                ClearCarriedProduct();
                onLeft?.Invoke(this);
            }));
    }
}
