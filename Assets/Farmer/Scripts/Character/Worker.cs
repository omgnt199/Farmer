using Farmer;
using UnityEngine;
public class Worker : Character
{
    protected override bool TryTakeNextTask(out CharacterTask task)
    {
        return base.TryTakeNextTask(out task) || CharacterTaskSystem.TryClaim(out task);
    }

    public static void QueueHarvestAndDelivery(ConstructionMono construction, Transform market)
    {
        if (construction == null)
            return;

        CharacterTaskSystem.Enqueue(new CharacterTask(
            CharacterTaskType.Harvest,
            construction.HarvestPoint,
            GameManager.SkipConstructionHarvestSpeed
                ? 0f
                : construction.ConstructionEntity.GetHarvestDuration(),
            CharacterState.Harvesting,
            _ =>
            {
                HarvestItem item = construction.Harvest();
                CharacterTaskSystem.Enqueue(new CharacterTask(
                    CharacterTaskType.Deliver,
                    market,
                    0f,
                    CharacterState.Delivering,
                    __ => MarketInventory.Store(item)));
            }));
    }
}
