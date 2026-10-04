using System.Collections.Generic;
using UnityEngine;

public sealed class CustomerQueue : MonoBehaviour
{
    [SerializeField] private List<MarketQueuePoint> queuePoints = new();
    private readonly Queue<Customer> customers = new();
    private readonly Dictionary<Customer, MarketQueuePoint> customerPoints = new();
    private readonly HashSet<MarketQueuePoint> occupiedPoints = new();
    private readonly HashSet<Customer> arrivedCustomers = new();

    public int Count => customers.Count;
    public int Capacity => queuePoints.Count;
    public int AvailableCount => queuePoints.Count - occupiedPoints.Count;

    public void Configure(Transform queueRoot)
    {
        queuePoints.Clear();
        if (queueRoot == null)
            return;

        var points = queueRoot.GetComponentsInChildren<MarketQueuePoint>(true);
        foreach (var point in points)
        {
            if (point.IsConfigured)
                queuePoints.Add(point);
        }
    }

    public bool TryEnqueue(Customer customer)
    {
        if (customer == null || AvailableCount == 0)
            return false;

        MarketQueuePoint point = null;
        foreach (var candidate in queuePoints)
        {
            if (occupiedPoints.Contains(candidate))
                continue;

            point = candidate;
            break;
        }

        if (point == null)
            return false;

        customerPoints.Add(customer, point);
        occupiedPoints.Add(point);
        customer.MoveToQueue(point.Queue, OnCustomerArrived);
        customers.Enqueue(customer);
        return true;
    }

    private void OnCustomerArrived(Customer customer)
    {
        if (customer != null && customerPoints.ContainsKey(customer))
            arrivedCustomers.Add(customer);
    }

    public bool TryDequeue(out Customer customer, out MarketQueuePoint point)
    {
        while (customers.Count > 0)
        {
            customer = customers.Peek();
            if (customer == null)
            {
                customers.Dequeue();
                arrivedCustomers.Remove(customer);
                if (customerPoints.TryGetValue(customer, out var stalePoint))
                {
                    customerPoints.Remove(customer);
                    if (stalePoint != null)
                        occupiedPoints.Remove(stalePoint);
                }
                continue;
            }

            if (!arrivedCustomers.Remove(customer))
            {
                customer = null;
                point = null;
                return false;
            }

            customers.Dequeue();
            customerPoints.TryGetValue(customer, out point);
            return true;
        }

        customer = null;
        point = null;
        return false;
    }

    public void Release(Customer customer)
    {
        if (customer == null || !customerPoints.TryGetValue(customer, out var point))
            return;

        arrivedCustomers.Remove(customer);
        customerPoints.Remove(customer);
        occupiedPoints.Remove(point);
    }
}
