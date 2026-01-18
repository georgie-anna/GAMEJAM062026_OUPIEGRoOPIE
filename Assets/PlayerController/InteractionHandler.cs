using UnityEngine;

/// <summary>
/// Behandelt Interaktionen mit Objekten
/// WICHTIG: Erbt NICHT von MonoBehaviour, da es keine Component ist!
/// </summary>
public class InteractionHandler
{
    public void TryInteract(GameObject target, GameObject interactor)
    {
        if (target == null)
            return;

        if (target.TryGetComponent<ICollectible>(out var collectable))
        {
            collectable.Collect(interactor);
        }
    }
}