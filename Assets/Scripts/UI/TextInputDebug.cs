using UnityEngine;

public class TextInputDebug : MonoBehaviour
{
    [SerializeField] private RealtimeAPIWrapper api;
    [SerializeField] private KeyCode submitKey = KeyCode.Return;

    private string inputText = "";
    private Vector2 scrollPos;
    private bool focusNextFrame = true;

    void OnGUI()
    {
        const float w = 460f;
        const float h = 120f;
        float x = 10f;
        float y = Screen.height - h - 10f;

        GUI.Box(new Rect(x, y, w, h), "Doctor — Text Input (Enter to send)");

        GUI.SetNextControlName("DoctorTextInput");
        inputText = GUI.TextField(new Rect(x + 10, y + 30, w - 110, 25), inputText, 500);

        if (focusNextFrame)
        {
            GUI.FocusControl("DoctorTextInput");
            focusNextFrame = false;
        }

        bool clicked = GUI.Button(new Rect(x + w - 90, y + 30, 80, 25), "Send");
        bool keyPressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == submitKey;

        if ((clicked || keyPressed) && !string.IsNullOrWhiteSpace(inputText))
        {
            if (api == null) api = FindObjectOfType<RealtimeAPIWrapper>();
            if (api != null)
            {
                api.SendUserMessageWithFullResponse(inputText.Trim());
                Debug.Log($"[TextInputDebug] sent: {inputText}");
                inputText = "";
                focusNextFrame = true;
            }
            else
            {
                Debug.LogWarning("[TextInputDebug] RealtimeAPIWrapper not found.");
            }
        }

        GUI.Label(new Rect(x + 10, y + 65, w - 20, 40), "Tip: 在按 Connect 之後使用。文字輸入會觸發完整 audio + animation 回覆。");
    }
}
