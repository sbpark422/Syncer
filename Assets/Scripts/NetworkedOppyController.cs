using UnityEngine;
using Fusion;
using OpenBCI.Network.Streams;
using UnityEngine.UI;

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

    [Header("Coin Collection")]
    [SerializeField] private Text coinCountText;
    private int coinCount = 0;  // Changed to regular int instead of networked

    [Header("Jump Thresholds")]
    [SerializeField] private float basicJumpThreshold = 0.5f;    // For Jump
    [SerializeField] private float jump2Threshold = 0.6f;        // For Jump2
    [SerializeField] private float ninjaJumpThreshold = 0.7f;    // For Ninja Jump

    // Add debug UI elements
    [Header("Debug")]
    [SerializeField] private bool showDebugUI = true;
    [SerializeField] private Text debugText; // Optional UI Text component

    // Add testing mode
    [Header("Testing")]
    [SerializeField] private bool useTestMode = true;
    [SerializeField] private float testMinInterval = 0.5f;
    [SerializeField] private float testMaxInterval = 2f;
    private float nextTestChangeTime;
    private float testAlphaValue;

    // States matching animator
    private enum JumpState
    {
        Running,    // Uses Run state (1102932609354020624)
        Jump,       // Uses Jump state (1102428337391504520)
        Jump2,      // Uses Jump2 state (1102953793907963858)
        NinjaJump   // Uses Ninja Jump state (1102850672355722210)
    }

    [Networked] 
    private JumpState CurrentJumpState { get; set; }
    
    [Networked]
    private NetworkBool IsRunning { get; set; }

    [Networked] 
    private float CurrentSpeed { get; set; }  // Changed from NetworkFloat to float

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
    }

    public override void Spawned()
    {
        // Initialize networked properties after spawning
        if (Object.HasStateAuthority)
        {
            CurrentJumpState = JumpState.Running;
            IsRunning = false;
            CurrentSpeed = 0f;
        }
        
        // Set initial animation state and position
        //_animator.SetBool("Running", false);
        
        if (maintainFixedPosition)
        {
            transform.position = fixedPosition;
        }

        // Initialize UI
        coinCount = 0;  // Reset local counter
        UpdateCoinUI();
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
            
            // Debug logging
            Debug.Log($"Alpha1: {bandPowerStream1?.AverageBandPower.Alpha:F2}, " +
                     $"Alpha2: {bandPowerStream2?.AverageBandPower.Alpha:F2}, " +
                     $"Avg: {avgAlpha:F2}, " +
                     $"State: {CurrentJumpState}");

            if (debugText != null)
            {
                debugText.text = $"Alpha1: {bandPowerStream1?.AverageBandPower.Alpha:F2}\n" +
                                $"Alpha2: {bandPowerStream2?.AverageBandPower.Alpha:F2}\n" +
                                $"Avg: {avgAlpha:F2}\n" +
                                $"State: {CurrentJumpState}";
            }

            // Always running when grounded
            if (CurrentJumpState == JumpState.Running)
            {
                IsRunning = true;
                CurrentSpeed = maxSpeed;
                //_animator.SetBool("Running", true);

                // Check for jumps
                UpdateJumpState(avgAlpha);
            }
            else
            {
                UpdateJumpState(avgAlpha);
            }
        }

        // Apply animations
        // _animator.SetBool("Running", IsRunning || CurrentJumpState == JumpState.Running);
        // _animator.SetFloat("Speed", CurrentSpeed);
        ApplyJumpAnimations();

    }

    private float CalculateAverageAlpha()
    {
        if (useTestMode)
        {
            // Update test alpha value at intervals
            if (Time.time >= nextTestChangeTime)
            {
                testAlphaValue = Random.value;  // Random value between 0-1
                nextTestChangeTime = Time.time + Random.Range(testMinInterval, testMaxInterval);
                
                // Debug log the new test value
                Debug.Log($"Test Alpha Value: {testAlphaValue:F2}");
            }
            return testAlphaValue;
        }

        // Original alpha calculation
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
        if (CurrentJumpState == JumpState.Running)
        {
            string jumpType = "None";
            
            // Choose jump type based on alpha level
            if (avgAlpha >= ninjaJumpThreshold)
            {
                CurrentJumpState = JumpState.NinjaJump;
                _animator.SetTrigger("NinjaJump");
                jumpType = "Ninja Jump";
            }
            else if (avgAlpha >= jump2Threshold)
            {
                CurrentJumpState = JumpState.Jump2;
                _animator.SetTrigger("Jump2");
                jumpType = "Jump2";
            }
            else if (avgAlpha >= basicJumpThreshold)
            {
                CurrentJumpState = JumpState.Jump;
                _animator.SetTrigger("Jump");
                jumpType = "Basic Jump";
            }

            if (jumpType != "None")
            {
                Debug.Log($"Triggered {jumpType} with alpha {avgAlpha:F2}");
            }
        }
        else
        {
            CurrentJumpState = JumpState.Running;
            Debug.Log("Returned to Running");
        }
    }

    private void ApplyJumpAnimations()
    {
        // This can be removed if we're handling all animations in UpdateJumpState
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Coins"))
        {
            // Use local counter instead of networked
            coinCount++;
            
            // Update UI
            UpdateCoinUI();

            // Simply destroy the coin
            Destroy(other.gameObject);
        }
    }

    private void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = $"Coins: {coinCount}";
        }
    }
} 