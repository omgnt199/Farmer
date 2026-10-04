using System;

public static class CustomerPopulation
{
    public static int Capacity { get; private set; } = 1;
    public static event Action<int> CapacityChanged;

    public static void AddCapacity(int amount)
    {
        if (amount <= 0)
            return;

        Capacity += amount;
        CapacityChanged?.Invoke(Capacity);
    }

    public static void Reset(int capacity = 1)
    {
        Capacity = Math.Max(1, capacity);
        CapacityChanged?.Invoke(Capacity);
    }
}
