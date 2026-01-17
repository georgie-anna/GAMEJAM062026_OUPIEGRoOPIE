using UnityEngine;

public class TargetProvider : MonoBehaviour
{
    //--- Depedndencies ---

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private LayerMask _targetLayerMask;
    [Range(0,10)]
    [SerializeField] private float _maxTargetDistance = 100f;


    //--- Public Methods ---
    public Collider GetTarget()
    {
        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hit, _maxTargetDistance, _targetLayerMask))
        {
            Collider target = hit.collider;
            return target;
        }

        else
        {
            return null;
        }
    }
}
