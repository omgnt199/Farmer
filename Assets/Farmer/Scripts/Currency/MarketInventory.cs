using System.Collections.Generic;

namespace Farmer
{
    public static class MarketInventory
    {
        private static readonly Queue<HarvestItem> Items = new();

        public static int Count => Items.Count;

        public static void Store(HarvestItem item)
        {
            Items.Enqueue(item);
        }

        public static bool TryTake(out HarvestItem item)
        {
            if (Items.Count > 0)
            {
                item = Items.Dequeue();
                return true;
            }

            item = default;
            return false;
        }

        public static void Clear()
        {
            Items.Clear();
        }
    }
}
