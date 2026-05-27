using UnityEditor;
using UnityEngine;

public class RealtimeAPIConnection : MonoBehaviour
{
    public static RealtimeAPIConnection instance;
    [HideInInspector] public bool isConnected = false;

    public string connectionStatus => isConnected ? "Connected" : "Disconnected";
    public string connectionButtonString => isConnected ? "Disconnect" : "Connect";

    private RealtimeAPIWrapper realtimeWrapper;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        realtimeWrapper = GetComponent<RealtimeAPIWrapper>();
    }

    void OnEnable()
    {
        RealtimeAPIWrapper.OnWebSocketConnected += HandleConnected;
        RealtimeAPIWrapper.OnWebSocketClosed += HandleClosed;
    }

    void OnDisable()
    {
        RealtimeAPIWrapper.OnWebSocketConnected -= HandleConnected;
        RealtimeAPIWrapper.OnWebSocketClosed -= HandleClosed;
    }

    private void HandleConnected() => isConnected = true;
    private void HandleClosed() => isConnected = false;

    void Update()
    {
        // 1. Connect via controller (Y button)
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            realtimeWrapper.ConnectWebSocketButton();
        }
    }

    // 2. Connect via UI (Scene GUI)
    public void sceneGUIButtonPressed()
    {
        // in realtime API wrapper, this will toggle the connection status
        realtimeWrapper.ConnectWebSocketButton();
    }
}
