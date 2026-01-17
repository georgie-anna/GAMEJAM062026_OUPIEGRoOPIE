/*****************************************************************************
* Project : 3D Charakter Steuerung (K2, S2, S3)
* File    : MoveBehaviour
* Date    : xx.xx.2025
* Author  : Eric Rosenberg
*
* Description :
* *
* History :
* xx.xx.2025 ER Created
******************************************************************************/

using UnityEngine;

/// <summary>
/// walk / run
/// </summary>
public class MoveBehaviour
{
    private Rigidbody _rb;
    private MoveConfig _moveConfig;

    //--- Anpassbare Parameter ---
    public float MoveSpeedMultiplier = 1f;      // Multiplikator für normale Geschwindigkeit
    public float SprintSpeedMultiplier = 1f;    // Multiplikator für Sprint-Geschwindigkeit

    public MoveBehaviour(Rigidbody rb, MoveConfig moveConfig)
    {
        _rb = rb;
        _moveConfig = moveConfig;
    }

    public void Move(Vector2 moveInput, bool isGrounded, bool isSprinting)
    {
        Vector3 velocity = _rb.linearVelocity;
        if (isGrounded)
        {
            velocity = OnGround(moveInput, velocity, isSprinting);
        }
        else
        {
            velocity = InAir(moveInput, velocity);
        }
        _rb.linearVelocity = velocity;
    }

    private Vector3 OnGround(Vector2 moveInput, Vector3 currentVelocity, bool isSprinting)
    {
        Vector3 moveDir = (_rb.transform.forward * moveInput.y + _rb.transform.right * moveInput.x).normalized;

        // Mit Multiplikatoren
        float baseSpeed = isSprinting ? _moveConfig.SprintSpeed * SprintSpeedMultiplier : _moveConfig.MoveSpeed * MoveSpeedMultiplier;
        Vector3 targetVelocity = moveDir * baseSpeed;

        float accel = moveInput.sqrMagnitude > 0f ? _moveConfig.Acceleration : _moveConfig.Deceleration;
        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetVelocity.x, accel * Time.fixedDeltaTime);
        currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, targetVelocity.z, accel * Time.fixedDeltaTime);
        return currentVelocity;
    }

    private Vector3 InAir(Vector2 moveInput, Vector3 currentVelocity)
    {
        if (moveInput.sqrMagnitude <= 0f)
            return currentVelocity;

        Vector3 horizontal = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        Vector3 inputDir = (_rb.transform.forward * moveInput.y + _rb.transform.right * moveInput.x).normalized;
        float speed = horizontal.magnitude;

        if (speed < 0.1f)
        {
            horizontal = inputDir * _moveConfig.AirStartSpeed;
        }
        else
        {
            Vector3 currentDir = horizontal.normalized;
            float dot = Vector3.Dot(currentDir, inputDir);
            if (dot > 0f)
            {
                Vector3 newDir = Vector3.Slerp(currentDir, inputDir, _moveConfig.AirControll * Time.fixedDeltaTime);
                horizontal = newDir * speed;
            }
            else
            {
                speed = Mathf.MoveTowards(speed, 0f, _moveConfig.AirBrake * Time.fixedDeltaTime);
                horizontal = currentDir * speed;
            }
        }

        currentVelocity.x = horizontal.x;
        currentVelocity.z = horizontal.z;
        return currentVelocity;
    }
}