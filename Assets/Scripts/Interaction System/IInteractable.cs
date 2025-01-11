using UnityEngine;

public interface IInteractable
{
    bool CanInteract(GameObject interactor);
    void OnInteractionStart(InteractionData data);
    void OnInteractionEnd(InteractionData data);
    void OnInteractionUpdate(InteractionData data);
}