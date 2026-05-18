using UnityEngine;
using System;
using System.Text;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using OpenAI.Responses;
using OpenAI.Chat;
using OpenAI.Conversations;
using System.Threading.Tasks;

public class functionANDTool : MonoBehaviour
{
    [Header("OpenAI")]
    public string apiKey = "YOUR_API_KEY";
    public string model = "gpt-4o-mini";

    private ChatClient client;
    private List<ChatMessage> history;

    // =========================
    // Unity Entry
    // =========================
    async void Start()
    {
        client = new ChatClient(model: model, apiKey: apiKey);

        history = new List<ChatMessage>
        {
            new SystemChatMessage(
                "You are a helpful assistant. " +
                "When appropriate, you MUST use tools to express emotion or request food."
            )
        };

        await SendToGPT("Hello, respond and choose an emotion tool to express yourself.");
        await SendToGPT("What food do you think that rabbit like to eat ? respond a food, do not express your emotion");
    }

    // =========================
    // Main GPT Call
    // =========================
    public async Task SendToGPT(string userMsg)
    {
        history.Add(new UserChatMessage(userMsg));

        var options = new ChatCompletionOptions
        {
            Tools = { emotionTool, foodTool }
        };

        Debug.Log($"[GPT_FAT]: send: {userMsg}");

        ChatCompletion completion = await client.CompleteChatAsync(history, options);

        // =========================
        // TOOL CALLING RESULT
        // =========================
        if (completion.FinishReason == ChatFinishReason.ToolCalls)
        {
            foreach (var toolCall in completion.ToolCalls)
            {
                HandleToolCall(toolCall);
            }
        }
        else
        {
            Debug.Log("[GPT_FAT] " + completion.Content[0].Text);
        }
    }

    // =========================
    // Tool Dispatcher
    // =========================
    private void HandleToolCall(ChatToolCall toolCall)
    {
        string name = toolCall.FunctionName;

        using JsonDocument json = JsonDocument.Parse(toolCall.FunctionArguments.ToString());

        if (name == nameof(HandleEmotion))
        {
            string emotion = json.RootElement.GetProperty("emotion").GetString();
            HandleEmotion(emotion);
        }
        else if (name == nameof(HandleFood))
        {
            string food = json.RootElement.GetProperty("food").GetString();
            HandleFood(food);
        }
    }

    // =========================
    // TOOLS (SIMPLIFIED)
    // =========================
    private static void HandleEmotion(string emotion)
    {
        Debug.Log($"[GPT_FAT] AI chose emotion: {emotion}");
    }

    private static void HandleFood(string food)
    {
        Debug.Log($"[GPT_FAT] AI chose food: {food}");
    }

    // =========================
    // TOOL DEFINITIONS
    // =========================
    private static readonly ChatTool emotionTool = ChatTool.CreateFunctionTool(
        functionName: nameof(HandleEmotion),
        functionDescription: "Use this function to express current emotion.",
        functionParameters: BinaryData.FromString(@"
        {
            ""type"": ""object"",
            ""properties"": {
                ""emotion"": {
                    ""type"": ""string"",
                    ""enum"": [""Happy"", ""Sad"", ""Angry"", ""Laugh"", ""Frustrate""]
                }
            },
            ""required"": [""emotion""]
        }")
    );

    private static readonly ChatTool foodTool = ChatTool.CreateFunctionTool(
        functionName: nameof(HandleFood),
        functionDescription: "If user ask you for a food, use this tool to choose a food item",
        functionParameters: BinaryData.FromString(@"
        {
            ""type"": ""object"",
            ""properties"": {
                ""food"": {
                    ""type"": ""string"",
                    ""enum"": [""rice"", ""carrot"", ""lettuce"", ""egg"", ""meat""]
                }
            },
            ""required"": [""food""]
        }")
    );
}