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
            Debug.Log($"Received message: {message}");
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
            // Parse the incoming JSON message
            MatrixData matrixData = JsonConvert.DeserializeObject<MatrixData>(message);
            
            if (matrixData?.matrix != null)
            {
                if (sharedObject != null)
                {
                    // Process the matrix directly
                    sharedObject.ProcessMatrixInput(matrixData.matrix);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling message: {e.Message}");
        }
    }

    // Data structure for messages
    [Serializable]
    private class MatrixData
    {
        public float[,] matrix; // 3x3 matrix
        public string timestamp;
    }

    // Event that other scripts can subscribe to
    public event Action<string> OnMessageReceived;
} 