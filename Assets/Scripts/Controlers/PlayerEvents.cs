using System;
using UnityEngine;

public static class PlayerEvents
{
    public static event Action OnLookedAround;
    public static event Action OnCameraSwitched;
    public static event Action OnMoved;
    public static event Action OnBookInteracted;
    public static event Action OnOpenedBookAndInventory;
    public static event Action OnProceedDownRoad;

    public static void LookedAround() => OnLookedAround?.Invoke();
    public static void CameraSwitched() => OnCameraSwitched?.Invoke();
    public static void Moved() => OnMoved?.Invoke();
    public static void BookInteracted() => OnBookInteracted?.Invoke();
    public static void OpenedBookAndInventory() => OnOpenedBookAndInventory?.Invoke();
    public static void ProceedDownRoad() => OnProceedDownRoad?.Invoke();
}
