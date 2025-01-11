using UnityEngine;

public class InteractableFlower : InteractableBase
{
    private void Awake()
    {
        ItemName = "Flower";
    }
    public override void OnInteractionStart(InteractionData data)
    {
        base.OnInteractionStart(data);
    }
}