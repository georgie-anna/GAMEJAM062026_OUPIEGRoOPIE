/*****************************************************************************
* Project : 3D Charakter Steuerung (K2, S2, S3)
* File    : JumpBehaviour
* Date    : xx.xx.2025
* Author  : Eric Rosenberg
*
* Description :
* Jump Behaviour mit anpassbaren Feel-Einstellungen
*
* History :
* xx.xx.2025 ER Created
* 
* 
* 
******************************************************************************/



using UnityEngine;


/// <summary>
/// player jump / modifiable
/// </summary>
public class JumpBehaviour
{
    //--- Dependencies ---
    private readonly Rigidbody _rb;
    private readonly JumpConfig _jumpConfig;

    //--- Fields ---
    private bool _groundJumpAvailable = true;
    private bool _isJumpButtonHeld = false;

    //--- Anpassbare Parameter ---
    public float JumpForceMultiplier = 1.5f;     // jump higher
    public float FallMultiplier = 3f;            // falls faster
    public float LowJumpMultiplier = 2f;         // lower jump if button released

    public JumpBehaviour(Rigidbody rb, JumpConfig jumpConfig)
    {
        _rb = rb;
        _jumpConfig = jumpConfig;
    }

    public bool Jump(JumpStateData jumpData)
    {
        if (CanJumpOnGround(jumpData.IsGrounded, jumpData.IsCoyoteActive))
        {
            PerformJumpPhysic();
            _groundJumpAvailable = false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Rufe diese Methode in FixedUpdate auf für besseres Jump-Gefühl
    /// </summary>
    public void UpdateJumpPhysics(bool jumpButtonHeld)
    {
        _isJumpButtonHeld = jumpButtonHeld;

        if (_rb.linearVelocity.y < 0)
        {
            // falls down
            _rb.linearVelocity += Vector3.up * Physics.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.linearVelocity.y > 0 && !_isJumpButtonHeld)
        {
            // jump height
            _rb.linearVelocity += Vector3.up * Physics.gravity.y * (LowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Applies the vertical jump force to the Rigidbody with multiplier
    /// </summary>
    private void PerformJumpPhysic()
    {
        Vector3 currentVelocity = _rb.linearVelocity;
        currentVelocity.y = _jumpConfig.JumpForce * JumpForceMultiplier;
        _rb.linearVelocity = currentVelocity;
    }

    /// <summary>
    /// Resets the ground jump availability
    /// </summary>
    public void ResetJumpCountGround()
    {
        _groundJumpAvailable = true;
    }

    /// <summary>
    /// Determines whether a ground or coyote-time jump is allowed
    /// </summary>
    private bool CanJumpOnGround(bool isGrounded, bool isCoyoteAktive)
    {
        return _groundJumpAvailable && (isGrounded || isCoyoteAktive);
    }
}