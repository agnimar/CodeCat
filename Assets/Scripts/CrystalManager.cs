using UnityEngine;
using UnityEngine.Events;

public class CrystalManager : MonoBehaviour
{
    public static CrystalManager Instance { get; private set; }

    [Header("All Crystal Activations")]
    [SerializeField] private CrystalActivation[] allCrystals;

    [Header("Barrier Settings")]
    [SerializeField] private GameObject barrier;
    [SerializeField] private Animator barrierAnimator;
    [SerializeField] private UnityEvent onCrystalsCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple CrystalManager instances found. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    public void DeactivateAllCrystals()
    {
        foreach (CrystalActivation crystal in allCrystals)
        {
            if (crystal != null)
            {
                crystal.Deactivate();
            }
        }
    }
    public void ReactivateAllCrystals()
    {
        foreach (CrystalActivation crystal in allCrystals)
        {
            if (crystal != null)
            {
                crystal.Activate();
            }
        }
    }
    public void CheckCrystalsState()
    {
        bool allActivated = true;
        foreach (CrystalActivation crystal in allCrystals)
        {
            if (crystal != null)
            {
                Debug.Log("Crystal " + crystal.gameObject.name + " activated: " + crystal.IsActivated);
                if (!crystal.IsActivated)
                {
                    allActivated = false;
                    break;
                }
            }
        }
        Debug.Log("CheckCrystalsState: All crystals activated? " + allActivated);
        if (allActivated)
        {
            LowerBarrier();
            onCrystalsCompleted?.Invoke();
        }
    }

    private void LowerBarrier()
    {
        Debug.Log("LowerBarrier called");
        if (barrierAnimator != null)
        {
            barrierAnimator.SetTrigger("Unlock"); 
        }
        else
        {
            Debug.LogError("Barrier Animator is not assigned!");
        }
    }
}
