using System;
using Farmer;
using UnityEngine;

public sealed class Delivery : Character
{
    private ParticleSystem leftFootSmoke;
    private ParticleSystem rightFootSmoke;
    private ConstructionMono sourceConstruction;
    private HarvestItem waitingProduct;
    private bool hasWaitingProduct;

    public ConstructionMono WaitingConstruction => sourceConstruction;

    public void AttachFootSmoke(ParticleSystem prefab)
    {
        if (prefab == null)
            return;

        Transform leftFoot = null;
        Transform rightFoot = null;
        foreach (var child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "foot_l")
                leftFoot = child;
            else if (child.name == "foot_r")
                rightFoot = child;

            if (leftFoot != null && rightFoot != null)
                break;
        }

        leftFootSmoke = CreateFootSmoke(prefab, leftFoot);
        rightFootSmoke = CreateFootSmoke(prefab, rightFoot);
    }

    private ParticleSystem CreateFootSmoke(ParticleSystem prefab, Transform foot)
    {
        if (foot == null)
            return null;

        var smoke = Instantiate(prefab, foot, false);
        smoke.transform.localPosition = Vector3.zero;
        smoke.transform.localRotation = Quaternion.identity;
        smoke.transform.localScale = Vector3.one;

        // var main = smoke.main;
        // main.simulationSpace = ParticleSystemSimulationSpace.World;
        // main.maxParticles = 24;
        // main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);

        var emission = smoke.emission;
        emission.enabled = false;
        return smoke;
    }

    private void LateUpdate()
    {
        bool isMoving = Agent != null && Agent.velocity.sqrMagnitude > 0.04f;
        UpdateFootSmoke(leftFootSmoke, isMoving);
        UpdateFootSmoke(rightFootSmoke, isMoving);
    }

    private void UpdateFootSmoke(ParticleSystem smoke, bool isMoving)
    {
        if (smoke == null)
            return;

        smoke.transform.rotation = transform.rotation;
        var emission = smoke.emission;
        emission.enabled = isMoving;
    }

    public void CollectAndWait(
        ConstructionMono construction,
        Transform deliveryEnd,
        Action<Delivery> onReady,
        Action<ConstructionMono> onAborted,
        Action<Delivery> onFinished)
    {
        if (construction == null)
        {
            onFinished?.Invoke(this);
            return;
        }

        sourceConstruction = construction;
        EnqueueTask(new CharacterTask(
            CharacterTaskType.MoveToConstruction,
            construction.HarvestPoint,
            0f,
            CharacterState.Delivering,
            _ =>
            {
                var productStartPosition = construction.ProductVisualStartPosition;
                if (!construction.TryTakeReadyProduct(out var product))
                {
                    onAborted?.Invoke(construction);
                    sourceConstruction = null;
                    ReturnToMarket(deliveryEnd, onFinished);
                    return;
                }

                SetCarryingProduct(product.ProductPrefab, productStartPosition, () =>
                {
                    waitingProduct = product;
                    hasWaitingProduct = true;
                    onReady?.Invoke(this);
                });
            }));
    }

    public bool StartDeliveryToCustomer(
        MarketQueuePoint marketPoint,
        Transform deliveryEnd,
        Action<ConstructionMono> onConstructionCleared,
        Action<HarvestItem> onDelivered,
        Action<Delivery> onFinished)
    {
        if (!hasWaitingProduct || marketPoint == null || marketPoint.DeliveryPoint == null)
            return false;

        HarvestItem product = waitingProduct;
        ConstructionMono construction = sourceConstruction;
        waitingProduct = default;
        hasWaitingProduct = false;
        sourceConstruction = null;

        EnqueueTask(new CharacterTask(
            CharacterTaskType.Deliver,
            marketPoint.DeliveryPoint,
            0.5f,
            CharacterState.Delivering,
            _ =>
            {
                onConstructionCleared?.Invoke(construction);
                onDelivered?.Invoke(product);
                ClearCarriedProduct();
                ReturnToMarket(deliveryEnd, onFinished);
            }));
        return true;
    }

    private void ReturnToMarket(Transform deliveryEnd, Action<Delivery> onFinished)
    {
        if (deliveryEnd == null)
        {
            onFinished?.Invoke(this);
            return;
        }

        EnqueueTask(new CharacterTask(
            CharacterTaskType.Leave,
            deliveryEnd,
            0f,
            CharacterState.Leaving,
            _ => onFinished?.Invoke(this)));
    }
}
