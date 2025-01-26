using UnityEngine;
using Fusion;
using OpenBCI.Network.Streams;

public class NetworkedAlphaPillar : NetworkBehaviour
{
    public GameObject alphaPillar;  // Reference to the visual pillar object

    public GameObject betaPillar;  // Reference to the visual pillar object
    public GameObject focusPillar;  // Reference to the visual pillar object
    [SerializeField] private AverageBandPowerStream Stream;

   [SerializeField] private FocusStream focusStream;
    [Networked]
    private float NetworkedAlphaPillarHeight { get; set; }

    [Networked]
    private float NetworkedBetaPillarHeight { get; set; }

    [Networked]
    private float NetworkedFocusPillarHeight { get; set; }



    public override void FixedUpdateNetwork()
    {
        // Only the StateAuthority should update the height
        if (Object.HasStateAuthority)
        {
            // Update networked height from stream
            NetworkedAlphaPillarHeight = Stream.AverageBandPower.Alpha;
            NetworkedBetaPillarHeight = Stream.AverageBandPower.Beta;
            NetworkedFocusPillarHeight = focusStream.Focus;
        }

        // All clients update their visual representation
        alphaPillar.transform.localScale = new Vector3(1, NetworkedAlphaPillarHeight, 1);
        betaPillar.transform.localScale = new Vector3(1, NetworkedBetaPillarHeight, 1);
        focusPillar.transform.localScale = new Vector3(1, NetworkedFocusPillarHeight, 1);

        // Optional debug
        Debug.Log($"Alpha: {NetworkedAlphaPillarHeight}");
        Debug.Log($"Beta: {NetworkedBetaPillarHeight}");
        Debug.Log($"Focus: {NetworkedFocusPillarHeight}");
    }
}
