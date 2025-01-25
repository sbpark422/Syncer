using Fusion;
using UnityEngine;

public class ColocationObjectController : NetworkBehaviour
{
    [Networked]
    private Vector3 NetworkedPosition { get; set; }
    [Networked]
    private Vector3 NetworkedVelocity { get; set; }

    private Vector3 previousVelocity;
    private float lastMessageTime;
    private bool isInitialized = false;

    //[SerializeField] private float accelerationRate = 5f;  // Units per second squared
    //[SerializeField] private float drag = 0.5f; 

    public override void Spawned()
    {
        Debug.Log($"Object spawned. HasStateAuthority: {Object.HasStateAuthority}");
        // Initialize when the object is spawned on the network
        if (Object.HasStateAuthority)
        {
            NetworkedPosition = transform.position;
            NetworkedVelocity = Vector3.zero;
            Debug.Log("Initialized networked properties");
        }
        previousVelocity = Vector3.zero;
        lastMessageTime = Time.time;
        isInitialized = true;
    }

    public void ProcessMatrixInput(float[,] matrix)
    {

        // Debug print the entire matrix
        string matrixString = "Received Matrix:\n";
        for (int i = 0; i < 3; i++)
        {
            matrixString += "[";
            for (int j = 0; j < 3; j++)
            {
                matrixString += $" {matrix[i,j]} ";
            }
            matrixString += "]\n";
        }
        Debug.Log(matrixString);


        
        if (!isInitialized)
        {
            Debug.LogWarning("Object not yet initialized");
            return;
        }

        if (matrix == null)
        {
            Debug.LogError("Received null matrix");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            Debug.LogWarning("No state authority");
            return;
        }

        // Calculate actual dt based on time since last message
        float currentTime = Time.time;
        float dt = currentTime - lastMessageTime;
        lastMessageTime = currentTime;

        // Initialize direction vector
        Vector3 direction = Vector3.zero;

        // First row: X axis (Left/Right)
        if (matrix[0, 0] == 1) // Stop
        {
            direction.x = 0;
        }
        else if (matrix[0, 1] == 1) // Left
        {
            direction.x = -1;
        }
        else if (matrix[0, 2] == 1) // Right
        {
            direction.x = 1;
        }

        // Second row: Z axis (Forward/Back)
        if (matrix[1, 0] == 1) // Stop
        {
            direction.z = 0;
        }
        else if (matrix[1, 1] == 1) // Forward
        {
            direction.z = 1;
        }
        else if (matrix[1, 2] == 1) // Back
        {
            direction.z = -1;
        }

        // Third row: Y axis (Up/Down)
        if (matrix[2, 0] == 1) // Stop
        {
            direction.y = 0;
        }
        else if (matrix[2, 1] == 1) // Up
        {
            direction.y = 1;
        }
        else if (matrix[2, 2] == 1) // Down
        {
            direction.y = -1;
        }

        // if (direction == Vector3.zero)  // When stopping
        // {
        //     NetworkedVelocity = Vector3.Lerp(previousVelocity, Vector3.zero, drag * dt);
        // }

        // Euler method for velocity and position
        // v(t) = v(t-1) + a(t)*dt
        // where a(t) is our direction input
        NetworkedVelocity = previousVelocity + direction * dt;
        previousVelocity = NetworkedVelocity;

        // x(t) = x(t-1) + v(t)*dt
        NetworkedPosition += NetworkedVelocity * dt;

        Debug.Log($"dt: {dt}, direction: {direction}, velocity: {NetworkedVelocity}");
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Apply position to transform
            transform.position = NetworkedPosition;
        }
    }

    // Reset method if needed
    public void ResetObject()
    {
        NetworkedPosition = Vector3.zero;
        NetworkedVelocity = Vector3.zero;
        previousVelocity = Vector3.zero;
    }
} 