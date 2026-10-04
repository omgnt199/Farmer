using System.Collections.Generic;

public static class CharacterTaskSystem
{
    private static readonly Queue<CharacterTask> PendingTasks = new();

    public static int PendingCount => PendingTasks.Count;

    public static void Enqueue(CharacterTask task)
    {
        if (task != null)
            PendingTasks.Enqueue(task);
    }

    public static bool TryClaim(out CharacterTask task)
    {
        if (PendingTasks.Count > 0)
        {
            task = PendingTasks.Dequeue();
            return true;
        }

        task = null;
        return false;
    }

    public static void Clear() => PendingTasks.Clear();
}
