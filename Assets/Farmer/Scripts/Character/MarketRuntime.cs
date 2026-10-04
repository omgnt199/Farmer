using System.Collections.Generic;
using Farmer;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MarketRuntime : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private GameObject deliveryPrefab;
    [SerializeField] private ParticleSystem footSmokePrefab;
    [SerializeField] private float buyDuration = 0.5f;

    private readonly List<Customer> customers = new();
    private readonly HashSet<ConstructionMono> claimedConstructions = new();
    private readonly Queue<Delivery> waitingDeliveries = new();

    private CustomerQueue customerQueue;
    private Transform customerStart;
    private Transform customerEnd;
    private Transform deliveryEnd;

    void Start()
    {
        customerStart = transform.Find("CustomerStart");
        customerEnd = transform.Find("CustomerEnd");
        deliveryEnd = transform.Find("DeliveryEnd");

        var dock = transform.Find("Dock");
        customerQueue = GetComponent<CustomerQueue>() ?? gameObject.AddComponent<CustomerQueue>();
        customerQueue.Configure(dock);

        if (customerPrefab == null || deliveryPrefab == null || customerStart == null ||
            customerEnd == null || deliveryEnd == null || customerQueue.Capacity == 0)
        {
            Debug.LogError("Market is missing a prefab or navigation marker.", this);
            enabled = false;
            return;
        }

        CustomerPopulation.CapacityChanged += OnCustomerCapacityChanged;
        ConstructionMono.ProductReady += SpawnDelivery;
        FillCustomerSlots();
    }

    void Update()
    {
        RemoveDestroyedCustomers();
        FillCustomerSlots();
        DispatchPendingDeliveries();
    }

    void OnDestroy()
    {
        CustomerPopulation.CapacityChanged -= OnCustomerCapacityChanged;
        ConstructionMono.ProductReady -= SpawnDelivery;
    }

    private void OnCustomerCapacityChanged(int _) => FillCustomerSlots();

    private void FillCustomerSlots()
    {
        while (customers.Count < CustomerPopulation.Capacity && customerQueue.AvailableCount > 0)
        {
            var instance = Instantiate(customerPrefab, customerStart.position, customerStart.rotation);
            var customer = instance.GetComponent<Customer>() ?? instance.AddComponent<Customer>();

            if (!customerQueue.TryEnqueue(customer))
            {
                Destroy(instance);
                return;
            }

            customers.Add(customer);
        }
    }

    private void OnCustomerLeft(Customer customer, MarketQueuePoint point)
    {
        customers.Remove(customer);
        customerQueue.Release(customer);
        if (customer != null)
            Destroy(customer.gameObject);

        FillCustomerSlots();
    }

    private void RemoveDestroyedCustomers()
    {
        for (int i = customers.Count - 1; i >= 0; i--)
        {
            if (customers[i] == null)
                customers.RemoveAt(i);
        }
    }

    private void SpawnDelivery(ConstructionMono construction)
    {
        if (construction == null || !construction.HasProduct || !claimedConstructions.Add(construction))
            return;

        var instance = Instantiate(deliveryPrefab, deliveryEnd.position, deliveryEnd.rotation);
        var delivery = instance.GetComponent<Delivery>() ?? instance.AddComponent<Delivery>();
        delivery.AttachFootSmoke(footSmokePrefab);
        delivery.CollectAndWait(
            construction,
            deliveryEnd,
            OnDeliveryReady,
            ReleaseConstruction,
            CompleteDelivery);
    }

    private void OnDeliveryReady(Delivery delivery)
    {
        if (delivery == null)
            return;

        waitingDeliveries.Enqueue(delivery);
        DispatchPendingDeliveries();
    }

    private void DispatchPendingDeliveries()
    {
        while (waitingDeliveries.Count > 0 && customerQueue.TryDequeue(out var customer, out var marketPoint))
        {
            var delivery = waitingDeliveries.Dequeue();
            if (delivery == null || customer == null || marketPoint == null)
            {
                if (customer != null)
                    customer.Leave(customerEnd, left => OnCustomerLeft(left, marketPoint));

                if (delivery != null)
                {
                    ReleaseConstruction(delivery.WaitingConstruction);
                    CompleteDelivery(delivery);
                }
                continue;
            }

            if (!delivery.StartDeliveryToCustomer(
                marketPoint,
                deliveryEnd,
                ReleaseConstruction,
                item =>
                {
                    customer.ReceiveProduct(
                        item,
                        buyDuration,
                        marketPoint.DeliveryPoint.position,
                        PayCustomerAt(item, marketPoint.Currency),
                        () => customer.Leave(customerEnd, left => OnCustomerLeft(left, marketPoint)));
                },
                CompleteDelivery))
            {
                customer.Leave(customerEnd, left => OnCustomerLeft(left, marketPoint));
                ReleaseConstruction(delivery.WaitingConstruction);
                CompleteDelivery(delivery);
            }
        }
    }

    private System.Action<HarvestItem> PayCustomerAt(HarvestItem item, Transform currencyPoint)
    {
        return _ =>
        {
            GameManager.Instance?.GameFX?.CoinEffect(currencyPoint.position);
            MarketPriceService.Sell(item);
        };
    }

    private void ReleaseConstruction(ConstructionMono construction)
    {
        claimedConstructions.Remove(construction);
        if (construction != null && construction.HasProduct)
            SpawnDelivery(construction);
    }

    private static void CompleteDelivery(Delivery delivery)
    {
        if (delivery != null)
            Destroy(delivery.gameObject);
    }

}
