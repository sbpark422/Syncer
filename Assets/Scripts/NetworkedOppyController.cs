using UnityEngine;
using Fusion;
using OpenBCI.Network.Streams;

public class NetworkedOppyController : NetworkBehaviour
{
    private Animator _animator;
    private CharacterController _characterController;

    [Header("Brain Data")]
    [SerializeField] private AverageBandPowerStream bandPowerStream1;
    [SerializeField] private AverageBandPowerStream bandPowerStream2;
    [SerializeField] private FocusStream focusStream;

    [Header("Thresholds")]
    [SerializeField] private float runThreshold = 0.3f;      // Start running at this alpha
    [SerializeField] private float jumpStartThreshold = 0.7f; // Start jumping at this alpha
    [SerializeField] private float floatThreshold = 0.5f;    
    [SerializeField] private float landThreshold = 0.3f;     

    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 0.3f;  // Minimum animation speed
    [SerializeField] private float maxSpeed = 1.0f;  // Maximum animation speed
    [SerializeField] private float speedMultiplier = 1.0f;  // Overall speed multiplier
    [SerializeField] private float speedSmoothTime = 0.1f;  // Time to reach target speed

    [Header("Auto Running")]
    [SerializeField] private float runDistance = 5f;     // Distance to run in each direction
    [SerializeField] private float runSpeed = 2f;        // Movement speed
    [SerializeField] private Vector3 runDirection = Vector3.forward;  // Direction to run

    [Header("Treadmill Effect")]
    [SerializeField] private Transform[] groundPlanes;  // Array of ground planes (usually 2)
    [SerializeField] private float groundScrollSpeed = 2f;
    [SerializeField] private float groundLength = 10f;  // Length of each ground plane

    [Header("Position")]
    [SerializeField] private Vector3 fixedPosition = new Vector3(0, 3f, 0);  // Raised Y position
    [SerializeField] private bool maintainFixedPosition = true;

    // Animation States
    private enum JumpState
    {
        Grounded,
        JumpStarted,
        Floating,
        Landing
    }

    [Networked] 
    private JumpState CurrentJumpState { get; set; }
    
    [Networked]
    private NetworkBool IsRunning { get; set; }

    [Networked] 
    private float CurrentSpeed { get; set; }  // Changed from NetworkFloat to float

    private float _currentVelocity;  // Used for smoothing
    private float _targetSpeed;      // Speed we're moving towards

    private Vector3 _startPosition;
    private bool _runningForward = true;
    private float _distanceTraveled = 0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Arrange ground planes initially
        if (groundPlanes != null && groundPlanes.Length >= 2)
        {
            // Position second ground plane right after the first one
            groundPlanes[1].position = groundPlanes[0].position + Vector3.forward * groundLength;
        }
    }

    public override void Spawned()
    {
        // Initialize networked properties after spawning
        if (Object.HasStateAuthority)
        {
            CurrentJumpState = JumpState.Grounded;
            IsRunning = false;
            CurrentSpeed = 0f;
        }
        
        // Set initial animation state and position
        _animator.SetBool("Running", false);
        
        if (maintainFixedPosition)
        {
            // Position Oppy above ground
            transform.position = fixedPosition;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Ensure Oppy stays in position
            if (maintainFixedPosition)
            {
                transform.position = fixedPosition;
            }

            float avgAlpha = CalculateAverageAlpha();

            // Scroll and loop ground planes
            if (groundPlanes != null && groundPlanes.Length >= 2)
            {
                // Move both grounds
                foreach (Transform ground in groundPlanes)
                {
                    ground.position += Vector3.back * groundScrollSpeed * Runner.DeltaTime;
                }

                // Check if first ground needs to loop
                if (groundPlanes[0].position.z <= -groundLength)
                {
                    groundPlanes[0].position = groundPlanes[1].position + Vector3.forward * groundLength;
                }

                // Check if second ground needs to loop
                if (groundPlanes[1].position.z <= -groundLength)
                {
                    groundPlanes[1].position = groundPlanes[0].position + Vector3.forward * groundLength;
                }
            }

            // Keep running animation going
            if (CurrentJumpState == JumpState.Grounded)
            {
                IsRunning = true;
                CurrentSpeed = maxSpeed;

                // Jump logic remains the same
                if (avgAlpha >= jumpStartThreshold)
                {
                    CurrentJumpState = JumpState.JumpStarted;
                }
            }
            else
            {
                UpdateJumpState(avgAlpha);
            }
        }

        // Apply animations
        _animator.SetBool("Running", IsRunning || CurrentJumpState == JumpState.Grounded);
        _animator.SetFloat("Speed", CurrentSpeed);
        ApplyJumpAnimations();

        if(focusStream != null && focusStream.Focus > 0.5f  )
        {
            _animator.SetBool("Running", true);
        }
    }

    private float CalculateAverageAlpha()
    {
        float totalAlpha = 0f;
        int validStreams = 0;

        if (bandPowerStream1 != null)
        {
            totalAlpha += bandPowerStream1.AverageBandPower.Alpha;
            validStreams++;
        }

        if (bandPowerStream2 != null)
        {
            totalAlpha += bandPowerStream2.AverageBandPower.Alpha;
            validStreams++;
        }

        return validStreams > 0 ? totalAlpha / validStreams : 0f;
    }

    private void UpdateJumpState(float avgAlpha)
    {
        switch (CurrentJumpState)
        {
            case JumpState.Grounded:
                if (avgAlpha >= jumpStartThreshold)
                {
                    CurrentJumpState = JumpState.JumpStarted;
                }
                break;

            case JumpState.JumpStarted:
                if (avgAlpha >= floatThreshold)
                {
                    CurrentJumpState = JumpState.Floating;
                }
                else if (avgAlpha <= landThreshold)
                {
                    CurrentJumpState = JumpState.Landing;
                }
                break;

            case JumpState.Floating:
                if (avgAlpha <= landThreshold)
                {
                    CurrentJumpState = JumpState.Landing;
                }
                break;

            case JumpState.Landing:
                CurrentJumpState = JumpState.Grounded;
                break;
        }
    }

    private void ApplyJumpAnimations()
    {
        switch (CurrentJumpState)
        {
            case JumpState.JumpStarted:
                _animator.SetTrigger("Jumping");
                break;
            case JumpState.Floating:
                _animator.SetTrigger("JumpIdle");
                break;
            case JumpState.Landing:
                _animator.SetTrigger("Landed");
                break;
        }
    }
} 