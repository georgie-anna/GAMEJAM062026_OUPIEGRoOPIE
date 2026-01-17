using UnityEngine;

public class TargetProvider : MonoBehaviour
{
    //--- Depedndencies ---

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private LayerMask _targetLayerMask;
    [Range(0,10)]
    [SerializeField] private float _maxTargetDistance = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
