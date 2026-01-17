using Unity.VisualScripting;
using UnityEngine;

public class InteracttionHandler : MonoBehaviour
{
    public void TryInteract(GameObject target, GameObject interactor)

    {
        if (target == null)
            return;

        if (target.TryGetComponent<ICollectible>(out var collectable))
        {
            collectable.collect(interactor);
        }
    }
}
