using UnityEngine;

public struct InteractionData
{
    public readonly GameObject Interactor;
    public readonly GameObject InteractedWith;
    public readonly Vector3 InteractionPoint;

    public InteractionData(GameObject interactor, GameObject interactedWith, Vector3 point)
    {
        Interactor = interactor;
        InteractedWith = interactedWith;
        InteractionPoint = point;
    }
}