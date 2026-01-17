using UnityEngine;

[CreateAssetMenu(fileName = "LookConfig", menuName = "Scriptable Objects/LookConfig")]
public class LookConfig : ScriptableObject
{
    [Header("Settings")]
    [Tooltip("Defines how sensitiv the lookmovement is.")]
    [Range(0, 1)]
    [SerializeField] private float _sensitivity = 1f;
    [SerializeField] private float _maxLookUp;
    [SerializeField] private float _minLookDown;


    public float Sensitivity => _sensitivity;

    public float MaxLookUp => _maxLookUp;

    public float MinLookDown => _minLookDown;

}
