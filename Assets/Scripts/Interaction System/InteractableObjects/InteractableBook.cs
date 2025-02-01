using UnityEngine;

public class InteractableBook : InteractableBase
{
    private void Awake()
    {
        ItemName = "Book";
    }
    public override void OnInteractionStart(InteractionData data)
    {
        onInteractionStarted?.Invoke(data);

        var itemGameObject = gameObject; 
        //TODO enable UI button to open book or smtn
        gameObject.SetActive(false); 
    }
    
}