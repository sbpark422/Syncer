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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
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
        
        // Set initial animation state
        _animator.SetBool("Running", false);
    }

    public override void FixedUpdateNetwork()
    {
        // if (Object.HasStateAuthority)
        // {
        //     float avgAlpha = CalculateAverageAlpha();
            
        //     // Handle running state
        //     if (CurrentJumpState == JumpState.Grounded)
        //     {
        //         // Run if alpha is above runThreshold but below jumpThreshold
        //         IsRunning = avgAlpha >= runThreshold && avgAlpha < jumpStartThreshold;
                
        //         if (IsRunning)
        //         {
        //             // Calculate target speed based on alpha
        //             float normalizedAlpha = Mathf.InverseLerp(runThreshold, jumpStartThreshold, avgAlpha);
        //             _targetSpeed = Mathf.Lerp(minSpeed, maxSpeed, normalizedAlpha) * speedMultiplier;
                    
        //             // Smoothly transition to target speed
        //             CurrentSpeed = Mathf.SmoothDamp(
        //                 current: CurrentSpeed,
        //                 target: _targetSpeed,
        //                 currentVelocity: ref _currentVelocity,
        //                 smoothTime: speedSmoothTime
        //             );
                    
        //             Debug.Log($"Target Speed: {_targetSpeed}, Current Speed: {CurrentSpeed}");
        //         }
                
        //         if (avgAlpha >= jumpStartThreshold)
        //         {
        //             IsRunning = false;
        //             CurrentJumpState = JumpState.JumpStarted;
        //             Debug.Log($"Jump Started! Alpha: {avgAlpha}");
        //         }
        //     }
        //     else
        //     {
        //         UpdateJumpState(avgAlpha);
        //     }
        // }

        // // Apply animations
        // _animator.SetBool("Running", IsRunning);
        // if (IsRunning)
        // {
        //     _animator.SetFloat("Speed", CurrentSpeed);  // Update animation speed
        // }
        // ApplyJumpAnimations();

        if(focusStream != null && focusStream.Focus > 0.5f)
        {
            Debug.Log($"Focus: {focusStream.Focus}");
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
                    Debug.Log($"Jump Started! Alpha: {avgAlpha}");
                }
                break;

            case JumpState.JumpStarted:
                if (avgAlpha >= floatThreshold)
                {
                    CurrentJumpState = JumpState.Floating;
                    Debug.Log($"Floating! Alpha: {avgAlpha}");
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
                    Debug.Log($"Landing! Alpha: {avgAlpha}");
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