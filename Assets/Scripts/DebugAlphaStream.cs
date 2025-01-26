using UnityEngine;
using OpenBCI.Network.Streams;

public class DebugAlphaStream : AverageBandPowerStream
{
    [SerializeField] private float minRandomInterval = 0.5f;
    [SerializeField] private float maxRandomInterval = 2f;
    
    private AverageBandPowerStream debugBandPower = new AverageBandPowerStream();
    private float nextChangeTime;

    private void Start()
    {
        SetNextChangeTime();
    }

    private void Update()
    {
        if (Time.time >= nextChangeTime)
        {
            // Generate random alpha value between 0 and 1
            debugBandPower.AverageBandPower.Alpha = Random.value;
            //AverageBandPowerStream = debugBandPower;
            
            SetNextChangeTime();
        }
    }

    private void SetNextChangeTime()
    {
        nextChangeTime = Time.time + Random.Range(minRandomInterval, maxRandomInterval);
    }
} 