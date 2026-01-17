/*****************************************************************************
* Project : 3D Character Controller (K2, S2, S3)
* File    : GroundCheck.cs
* Date    : xx.xx.2025
* Author  : Eric Rosenberg
*
* Description :
* Checks whether the player character is grounded using a box-shaped
* physics overlap test. Can optionally visualize the ground check area
* and state for debugging purposes.
*
* History :
* xx.xx.2025 ER Created
******************************************************************************/
using UnityEngine;

/// <summary>
/// Component that detects whether the object is grounded.
/// Uses a box overlap check against specified ground layers.
/// </summary>
public class GroundCheck : MonoBehaviour
{
    //--- Field ---
    [Header("Settings")]
    [Tooltip("Layers that are considered ground.")]
    [SerializeField] private LayerMask _layer;
    [Tooltip("Size of the ground check box.")]
    [SerializeField] private Vector3 _boxSize;
    [Tooltip("Offset position of the ground check box.")]
    [SerializeField] private Vector3 _boxOffset;
    [Tooltip("Enable debug visualization in the Scene view.")]
    [SerializeField] private bool DebugMode = false;

    private bool _debugIsGrounded;

    /// <summary>
    /// Updates the grounded state in debug mode.
    /// Uses FixedUpdate to stay in sync with the physics system.
    /// </summary>
    private void FixedUpdate()
    {
        if (!DebugMode) return;
        _debugIsGrounded = IsGrounded();
    }

    /// <summary>
    /// Checks whether the object is currently grounded.
    /// Performs a box overlap test against the configured ground layers.
    /// </summary>
    /// <returns>
    /// True if at least one collider is detected inside the ground check box;
    /// otherwise, false.
    /// </returns>
    public bool IsGrounded()
    {
        Vector3 boxPosition = transform.position + _boxOffset;
        Collider[] hit = Physics.OverlapBox(boxPosition, _boxSize, Quaternion.identity, _layer);
        return hit.Length > 0;
    }

    /// <summary>
    /// Draws the ground check gizmo in the Scene view when debug mode is enabled.
    /// The gizmo color indicates whether the object is grounded.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (DebugMode)
        {
            Vector3 position = transform.position + _boxOffset;

            Gizmos.color = Application.isPlaying && _debugIsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(position, _boxSize * 2);
        }
    }
}
