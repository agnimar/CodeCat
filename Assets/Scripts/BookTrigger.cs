using System;
using UnityEngine;

public class BookTrigger : MonoBehaviour
{
    public static event Action OnEnteredBookArea;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnEnteredBookArea?.Invoke();
        }
    }
}
