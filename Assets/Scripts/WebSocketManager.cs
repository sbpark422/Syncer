using UnityEngine;
using System;
using NativeWebSocket;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class WebSocketManager : MonoBehaviour
{
    private static WebSocketManager _instance;
    public static WebSocketManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("WebSocketManager");
                _instance = go.AddComponent<WebSocketManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private WebSocket _websocket;
    private readonly string _websocketUrl = "ws://localhost:8765";
    public bool IsConnected => _websocket?.State == WebSocketState.Open;

    // Add message type constants
    public const string MESSAGE_TYPE_COMMAND = "command";
    public const string MESSAGE_TYPE_RESPONSE = "response";

    public ColocationObjectController sharedObject; // Assign this in the Inspector

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        try
        {
            if (sharedObject == null)
            {
                Debug.LogError("SharedObject not assigned to WebSocketManager!");
                return;
            }
            
            await ConnectToServer();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to connect to server: {e.Message}");
        }
    }

    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
        if (_websocket != null)
        {
            _websocket.DispatchMessageQueue();
        }
        #endif
    }

    private async void OnApplicationQuit()
    {
        try
        {
            await DisconnectFromServer();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during disconnect: {e.Message}");
        }
    }

    private async void OnDestroy()
    {
        try
        {
            await DisconnectFromServer();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during destroy: {e.Message}");
        }
    }

    public async Task ConnectToServer()
    {
        _websocket = new WebSocket(_websocketUrl);

        _websocket.OnOpen += () =>
        {
            Debug.Log("Connection established!");
        };

        _websocket.OnError += (e) =>
        {
            Debug.LogError($"Error! {e}");
        };

        _websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        _websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log($"Raw message received: {message}");
            HandleMessage(message);
        };

        await _websocket.Connect();
    }

    private async Task DisconnectFromServer()
    {
        if (_websocket != null && _websocket.State == WebSocketState.Open)
        {
            await _websocket.Close();
        }
    }

    public async Task SendMessage(string message)
    {
        if (_websocket?.State == WebSocketState.Open)
        {
            await _websocket.SendText(message);
        }
        else
        {
            Debug.LogWarning("WebSocket is not connected!");
        }
    }

    private void HandleMessage(string message)
    {
        try
        {
            Debug.Log($"Attempting to parse message: {message}");
            
            // Parse the incoming JSON message
            MatrixData matrixData = JsonConvert.DeserializeObject<MatrixData>(message);
            
            if (matrixData == null)
            {
                Debug.LogError("Deserialized matrixData is null");
                return;
            }
            
            if (matrixData.matrix == null)
            {
                Debug.LogError("Matrix data is null");
                return;
            }

            Debug.Log($"Matrix dimensions: {matrixData.matrix.Length} x {matrixData.matrix[0].Length}");
            
            if (matrixData?.matrix != null)
            {
                // Convert jagged array to 2D array
                float[,] matrix2D = new float[3, 3];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        matrix2D[i, j] = matrixData.matrix[i][j];
                    }
                }

                if (sharedObject != null)
                {
                    // Process the converted matrix
                    sharedObject.ProcessMatrixInput(matrix2D);
                }
                else
                {
                    Debug.LogWarning("SharedObject is not assigned!");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling message: {e.Message}\nStack trace: {e.StackTrace}\nMessage content: {message}");
        }
    }

    // Data structure for messages
    [Serializable]
    private class MatrixData
    {
        public float[][] matrix; // Changed from float[,] to float[][]
        public string timestamp;
    }

    // Event that other scripts can subscribe to
    public event Action<string> OnMessageReceived;
} 