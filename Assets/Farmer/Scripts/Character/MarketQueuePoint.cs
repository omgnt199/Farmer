using UnityEngine;

public sealed class MarketQueuePoint : MonoBehaviour
{
    [SerializeField] private Transform deliveryPoint;
    [SerializeField] private Transform queue;
    [SerializeField] private Transform currency;

    public Transform DeliveryPoint => deliveryPoint;
    public Transform Queue => queue;
    public Transform Currency => currency;

    public bool IsConfigured => deliveryPoint != null && queue != null && currency != null;
}
