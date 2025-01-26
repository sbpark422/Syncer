using UnityEngine;
using Fusion;
using OpenBCI.Network.Streams;
using UnityEngine.UI;
using System.Collections;

public class NetworkedOppyController : NetworkBehaviour
{
    private Animator _animator;
    [SerializeField] private OppyLightGlow _oppyLightGlow;

    [Header("Brain Data")]
    [SerializeField] private AverageBandPowerStream bandPowerStream1;
    [SerializeField] private AverageBandPowerStream bandPowerStream2;

    [Header("Position")]
    [SerializeField] private Vector3 fixedPosition = new Vector3(0, 3f, 0);  // Raised Y position
    [SerializeField] private bool maintainFixedPosition = true;

    [Header("Jump Thresholds")]
    [SerializeField] private float basicJumpThreshold = 0.5f;    // For Jump
    [SerializeField] private float jump2Threshold = 0.6f;        // For Jump2
    [SerializeField] private float ninjaJumpThreshold = 0.7f;    // For Ninja Jump

    // Add debug UI elements
    [Header("Debug")]
    [SerializeField] private bool showDebugUI = true;
    [SerializeField] private Text debugText;

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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            CurrentJumpState = JumpState.Running;
        }
        
        if (maintainFixedPosition)
        {
            transform.position = fixedPosition;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
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

            UpdateJumpState(avgAlpha);
        }
    }

    private float CalculateAverageAlpha()
    {
        if (useTestMode)
        {
            if (Time.time >= nextTestChangeTime)
            {
                testAlphaValue = Random.value;
                nextTestChangeTime = Time.time + Random.Range(testMinInterval, testMaxInterval);
                Debug.Log($"Test Alpha Value: {testAlphaValue:F2}");
            }
            return testAlphaValue;
        }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Flash red and play failure sound
            if (_oppyLightGlow != null)
            {
                _oppyLightGlow.SetRed(true);
                StartCoroutine(ResetGlow());
            }
        }
    }

    private IEnumerator ResetGlow()
    {
        yield return new WaitForSeconds(0.5f);  // Flash duration
        if (_oppyLightGlow != null)
        {
            _oppyLightGlow.SetRed(false);
        }
    }
} 