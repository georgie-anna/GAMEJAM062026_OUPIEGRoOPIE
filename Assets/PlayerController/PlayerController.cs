/*****************************************************************************
* Project : 3D Charakter Steuerung (K2, S2, S3)
* File    : PlayerController
* Date    : xx.xx.2025
* Author  : Eric Rosenberg
*
* Description :
* Hauptsteuerung für den Spieler-Charakter
* 
* History :
* xx.xx.2025 ER Created
******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public struct JumpStateData
{
    public bool IsGrounded;
    public bool IsCoyoteActive;
    public bool MultiJumpEnabled;
}

public class PlayerController : MonoBehaviour
{
    //--- Dependencies ---
    [Header("Dependencies")]
    [Tooltip("Rigibody from Player")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GroundCheck _groundCheck;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private TargetProvider _targetProvider;
    [Tooltip("Animator für die Spielfigur (auf SK_Elf)")]
    [SerializeField] private Animator _animator;

    private InteractionHandler _interactionHandler;

    [Tooltip("MoveConfig Asset")]
    [SerializeField] private MoveConfig _moveConfig;
    [SerializeField] private JumpConfig _jumpConfig;
    [SerializeField] private LookConfig _lookConfig;

    [Tooltip("Activates Multi Jump")]
    [SerializeField] private bool _multiJumpEnabled = true;

    //--- Maus Einstellungen ---
    [Header("Maus Einstellungen")]
    [Tooltip("Multiplikator für horizontale Mausgeschwindigkeit (X-Achse, links/rechts)")]
    [SerializeField] private float _mouseSensitivityX = 5f;
    [Tooltip("Multiplikator für vertikale Mausgeschwindigkeit (Y-Achse, hoch/runter)")]
    [SerializeField] private float _mouseSensitivityY = 5f;
    [Tooltip("Maus verstecken und sperren?")]
    [SerializeField] private bool _hideMouse = true;

    //--- Movement Einstellungen ---
    [Header("Movement Einstellungen")]
    [Tooltip("Multiplikator für normale Laufgeschwindigkeit")]
    [SerializeField] private float _moveSpeedMultiplier = 1f;
    [Tooltip("Multiplikator für Sprint-Geschwindigkeit")]
    [SerializeField] private float _sprintSpeedMultiplier = 1.5f;

    //--- Jump Einstellungen ---
    [Header("Jump Einstellungen")]
    [Tooltip("Multiplikator für Sprungkraft (höher = springt höher)")]
    [SerializeField] private float _jumpForceMultiplier = 1.5f;
    [Tooltip("Wie schnell der Spieler runter fällt (höher = schneller)")]
    [SerializeField] private float _fallMultiplier = 3f;
    [Tooltip("Wie schnell Sprung stoppt wenn Taste losgelassen (höher = kürzere Sprünge)")]
    [SerializeField] private float _lowJumpMultiplier = 2f;

    private MoveBehaviour _moveBehaviour;
    private JumpBehaviour _jumpBehaviour;
    private LookBehaviour _lookBehaviour;

    //--- Fields ---
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private JumpStateData JumpData;

    private float _coyoteTimeCounter = 0f;
    private float _jumpBufferCounter = 0f;

    private bool _isGrounded = false;
    private bool _wasGrounded;
    private bool _isSprinting = false;

    private PlayerInputAction _inputAction;
    private InputAction _move;
    private InputAction _look;
    private InputAction _jump;
    private InputAction _sprint;
    private InputAction _interact;

    public bool JustLanded => !_wasGrounded && _isGrounded;
    public bool JustLeftGround => _wasGrounded && !_isGrounded;

    // Animation Parameter Namen
    private readonly int _speedHash = Animator.StringToHash("Speed");
    private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int _jumpHash = Animator.StringToHash("Jump");

    private void Awake()
    {
        MappingInputAction();
        InitializeData();
        _interactionHandler = new InteractionHandler();
    }

    private void OnEnable()
    {
        _inputAction.Player.Enable();
    }

    private void OnDisable()
    {
        _inputAction?.Player.Disable();
    }

    private void Update()
    {
        HandleCursorVisibility();

        _moveInput = _move.ReadValue<Vector2>();
        if (_jump.WasPressedThisFrame())
        {
            ResetJumpBufferTimer();
        }
        _lookInput = _look.ReadValue<Vector2>();

        SetIsSprinting();

        if (_interact.WasPressedThisFrame())
        {
            GameObject target = _targetProvider.GetTarget();
            _interactionHandler.TryInteract(target, gameObject);
        }

        // Update Animationen
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        UpdateGroundState();
        ReduceCoyoteTimer();
        ReduceJumpBuffer();
        HandleGroundTransition();
        HandleMovement(_isGrounded);
        HandleJump();
        HandleLook();

        // Better Jump Physics
        _jumpBehaviour.UpdateJumpPhysics(_jump.IsPressed());
    }

    /// <summary>
    /// Aktualisiert die Animationen basierend auf Spieler-Status
    /// </summary>
    private void UpdateAnimations()
    {
        if (_animator == null) return;

        // Berechne Bewegungsgeschwindigkeit (0 = Idle, 1 = Walk, 2 = Sprint)
        float speed = 0f;
        if (_moveInput.magnitude > 0.1f)
        {
            speed = _isSprinting ? 2f : 1f;
        }

        // Setze Animation Parameter
        _animator.SetFloat(_speedHash, speed);
        _animator.SetBool(_isGroundedHash, _isGrounded);
    }

    /// <summary>
    /// Triggert die Sprung-Animation
    /// </summary>
    private void TriggerJumpAnimation()
    {
        if (_animator == null) return;
        _animator.SetTrigger(_jumpHash);
    }

    private void HandleCursorVisibility()
    {
        bool isGameOverActive = false;
        if (GameOverUI.Instance != null && GameOverUI.Instance.gameObject.activeInHierarchy)
        {
            isGameOverActive = true;
        }

        if (_hideMouse && !isGameOverActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (isGameOverActive)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void EnableMouseControl()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableMouseControl()
    {
        if (_hideMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void HandleLook()
    {
        Vector2 adjustedLookInput = new Vector2(
            _lookInput.x * _mouseSensitivityX,
            _lookInput.y * _mouseSensitivityY
        );
        _lookBehaviour.Look(adjustedLookInput);
    }

    private void UpdateGroundState()
    {
        _wasGrounded = _isGrounded;
        _isGrounded = _groundCheck.IsGrounded();
    }

    private void ReduceCoyoteTimer()
    {
        if (!_isGrounded && _coyoteTimeCounter > 0f)
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void ResetCoyoteTimer()
    {
        _coyoteTimeCounter = _jumpConfig.CoyoteTime;
    }

    private bool IsCoyoteTimeActive()
    {
        return _coyoteTimeCounter > 0f;
    }

    private void ReduceJumpBuffer()
    {
        if (_jumpBufferCounter > 0f)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }

    private void ResetJumpBufferTimer()
    {
        _jumpBufferCounter = _jumpConfig.JumpBufferTime;
    }

    private void HandleGroundTransition()
    {
        if (JustLanded)
        {
            ResetGroundJumpCounter();
        }
        if (JustLeftGround)
        {
            ResetCoyoteTimer();
        }
    }

    private void HandleJump()
    {
        if (_jumpBufferCounter <= 0f)
            return;

        JumpData = BuildJumpData(JumpData);

        if (_jumpBehaviour.Jump(JumpData))
        {
            _jumpBufferCounter = 0f;
            TriggerJumpAnimation(); // Sprung-Animation triggern
        }
    }

    private JumpStateData BuildJumpData(JumpStateData JumpData)
    {
        JumpData.IsGrounded = _isGrounded;
        JumpData.IsCoyoteActive = IsCoyoteTimeActive();
        JumpData.MultiJumpEnabled = _multiJumpEnabled;

        return JumpData;
    }

    private void SetIsSprinting()
    {
        if (_sprint.IsPressed())
        {
            _isSprinting = true;
        }
        else
        {
            _isSprinting = false;
        }
    }

    private void ResetGroundJumpCounter()
    {
        _jumpBehaviour.ResetJumpCountGround();
    }

    private void MappingInputAction()
    {
        _inputAction = new();
        _move = _inputAction.Player.Move;
        _jump = _inputAction.Player.Jump;
        _sprint = _inputAction.Player.Sprint;
        _look = _inputAction.Player.Look;
        _interact = _inputAction.Player.Interact;
    }

    private void InitializeData()
    {
        _moveBehaviour = new(_rb, _moveConfig);
        _jumpBehaviour = new(_rb, _jumpConfig);
        _lookBehaviour = new(_rb, _lookConfig, _camTransform);
        JumpData = new JumpStateData();

        _moveBehaviour.MoveSpeedMultiplier = _moveSpeedMultiplier;
        _moveBehaviour.SprintSpeedMultiplier = _sprintSpeedMultiplier;

        _jumpBehaviour.JumpForceMultiplier = _jumpForceMultiplier;
        _jumpBehaviour.FallMultiplier = _fallMultiplier;
        _jumpBehaviour.LowJumpMultiplier = _lowJumpMultiplier;
    }

    private void HandleMovement(bool isGrounded)
    {
        _moveBehaviour.Move(_moveInput, isGrounded, _isSprinting);
    }
}