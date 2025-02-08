using System;

public static class InventoryEvents
{
    public static event Action OnBookCollected;

    public static void BookCollected()
    {
        OnBookCollected?.Invoke();
    }
}
