/*****************************************************************************
?
******************************************************************************/

using UnityEngine;


/// <summary>
/// camera zoom in and out
/// </summary>
public class CameraZoom : MonoBehaviour
{
    // modifiable zoom settings
    [Header("Zoom Einstellungen")]
    [Tooltip("Minimale Distanz zur Figur")]
    [SerializeField] private float _minDistance = 2f;

    [Tooltip("Maximale Distanz zur Figur")]
    [SerializeField] private float _maxDistance = 10f;

    [Tooltip("Aktuelle Start-Distanz")]
    [SerializeField] private float _currentDistance = 5f;

    [Tooltip("Wie schnell gezoomt wird")]
    [SerializeField] private float _zoomSpeed = 2f;

    [Tooltip("Wie smooth der Zoom ist (niedriger = glatter)")]
    [SerializeField] private float _zoomSmoothness = 0.1f;

    private float _targetDistance;
    private float _zoomVelocity;

    private void Start()
    {
        _targetDistance = _currentDistance;
    }

    private void Update()
    {
        HandleZoomInput();
    }

    private void LateUpdate()
    {
        ApplyZoom();
    }

    private void HandleZoomInput()
    {
        // Mausrad Input (positiv = rein, negativ = raus)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f)
        {
            _targetDistance -= scrollInput * _zoomSpeed;
            _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);
        }
    }

    private void ApplyZoom()
    {
        // smooth zoom
        _currentDistance = Mathf.SmoothDamp(_currentDistance, _targetDistance, ref _zoomVelocity, _zoomSmoothness);

        // set camera position
        Vector3 newPos = transform.localPosition;
        newPos.z = -_currentDistance;
        transform.localPosition = newPos;
    }
}