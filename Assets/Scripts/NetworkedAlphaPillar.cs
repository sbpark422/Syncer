using UnityEngine;
using Fusion;
using OpenBCI.Network.Streams;

public class NetworkedAlphaPillar : NetworkBehaviour
{
    public enum PillarMode
    {
        Alpha,
        Beta,
        Focus
    }

    public GameObject pillar;  // Reference to the visual pillar object
    
    [SerializeField] private AverageBandPowerStream bandPowerStream;
    [SerializeField] private FocusStream focusStream;
    
    [SerializeField] private PillarMode mode = PillarMode.Alpha;  // Dropdown in inspector

    [Networked]
    private float NetworkedPillarHeight { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Update height based on selected mode
            switch (mode)
            {
                case PillarMode.Focus:
                    if (focusStream != null)
                    {
                        NetworkedPillarHeight = focusStream.Focus;
                        Debug.Log($"Focus: {NetworkedPillarHeight}");
                    }
                    break;

                case PillarMode.Alpha:
                    if (bandPowerStream != null)
                    {
                        NetworkedPillarHeight = bandPowerStream.AverageBandPower.Alpha;
                        Debug.Log($"Alpha: {NetworkedPillarHeight}");
                    }
                    break;

                case PillarMode.Beta:
                    if (bandPowerStream != null)
                    {
                        NetworkedPillarHeight = bandPowerStream.AverageBandPower.Beta;
                        Debug.Log($"Beta: {NetworkedPillarHeight}");
                    }
                    break;
            }
        }

        // All clients update their visual representation
        pillar.transform.localScale = new Vector3(1, NetworkedPillarHeight, 1);
    }
}
