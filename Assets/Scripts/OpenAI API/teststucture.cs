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

public class teststructure : MonoBehaviour
{
    public string apiKey = "YOUR_API_KEY";
    public string model = "gpt-4o-mini";

    private ChatClient client;

    void Start()
    {
        client = new ChatClient(model: model, apiKey: apiKey);
        RunTest();
    }

    void RunTest()
    {
        List<ChatMessage> messages = new()
        {
            new SystemChatMessage(
                "You must respond ONLY in valid JSON.\n" +
                "Format:\n" +
                "{ \"mood\": string, \"reason\": string }\n" +
                "Do not include any extra text."
            ),
            new UserChatMessage("Tell me your mood and a short reason.")
        };

        Debug.Log("[GPT] Sending request...");

        try
        {
            ChatCompletion completion = client.CompleteChat(messages);

            string text = completion.Content[0].Text;
            Debug.Log("[Raw]: " + text);

            using JsonDocument doc = JsonDocument.Parse(text);

            string mood = doc.RootElement.GetProperty("mood").GetString();
            string reason = doc.RootElement.GetProperty("reason").GetString();

            Debug.Log($"Mood: {mood}");
            Debug.Log($"Reason: {reason}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[ERROR]: " + e.Message);
        }
    }
}

//public class teststucture : MonoBehaviour
//{
//    [Header("OpenAI Settings")]
//    public string apiKey = "YOUR_API_KEY";
//    public string model = "gpt-4.1";

//    private ChatClient client;

//    async void Start()
//    {
//        client = new ChatClient(
//            model: model,
//            apiKey: apiKey
//        );

//        await RunTest();
//    }

//    async Task RunTest()
//    {
//        Debug.Log("Starting Structured Output Test...");

//        List<ChatMessage> messages = new()
//        {
//            new UserChatMessage("How can I solve 8x + 7 = -23?")
//        };

//        // IMPORTANT:
//        // Unity old C# version cannot use """ raw string literal
//        string schema = @"
//        {
//            ""type"": ""object"",
//            ""properties"": {
//                ""steps"": {
//                    ""type"": ""array"",
//                    ""items"": {
//                        ""type"": ""object"",
//                        ""properties"": {
//                            ""explanation"": { ""type"": ""string"" },
//                            ""output"": { ""type"": ""string"" }
//                        },
//                        ""required"": [""explanation"", ""output""],
//                        ""additionalProperties"": false
//                    }
//                },
//                ""final_answer"": { ""type"": ""string"" }
//            },
//            ""required"": [""steps"", ""final_answer""],
//            ""additionalProperties"": false
//        }";

//        ChatCompletionOptions options = new()
//        {
//            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
//                jsonSchemaFormatName: "math_reasoning",
//                jsonSchema: BinaryData.FromString(schema),
//                jsonSchemaIsStrict: true
//            )
//        };

//        try
//        {
//            ChatCompletion completion = await client.CompleteChatAsync(
//                messages,
//                options
//            );

//            string jsonText = completion.Content[0].Text;

//            Debug.Log("===== RAW JSON =====");
//            Debug.Log(jsonText);

//            using JsonDocument structuredJson = JsonDocument.Parse(jsonText);

//            string finalAnswer =
//                structuredJson.RootElement
//                .GetProperty("final_answer")
//                .GetString();

//            Debug.Log("Final Answer: " + finalAnswer);

//            Debug.Log("===== STEPS =====");

//            foreach (JsonElement stepElement in structuredJson
//                .RootElement
//                .GetProperty("steps")
//                .EnumerateArray())
//            {
//                string explanation =
//                    stepElement.GetProperty("explanation").GetString();

//                string output =
//                    stepElement.GetProperty("output").GetString();

//                Debug.Log("Explanation: " + explanation);
//                Debug.Log("Output: " + output);
//            }

//            Debug.Log("Structured Output Test Success!");
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("[GPT_STU] Structured Output Test Failed!");
//            Debug.LogError($"[GPT_STU] {e.Message}");
//            Debug.LogError($"[GPT_STU] {e.StackTrace }");
//        }
//    }
//}


//public class teststucture : MonoBehaviour
//{
//    public string apiKey = "YOUR_API_KEY";
//    public string model = "gpt-4.1";

//    private ChatClient client;

//    async void Start()
//    {
//        client = new ChatClient(model: model, apiKey: apiKey);

//        await RunTest();
//    }

//    async Task RunTest()
//    {
//        List<ChatMessage> messages = new()
//        {
//            new SystemChatMessage(
//                "You must respond in valid JSON only.\n\n" +
//                "Format:\n" +
//                "{\n" +
//                "  \"steps\": [\n" +
//                "    {\n" +
//                "      \"explanation\": \"string\",\n" +
//                "      \"output\": \"string\"\n" +
//                "    }\n" +
//                "  ],\n" +
//                "  \"final_answer\": \"string\"\n" +
//                "}"
//            ),

//            new UserChatMessage("How can I solve 8x + 7 = -23?")
//        };

//        ChatCompletionOptions options = new()
//        {
//            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
//        };

//        try
//        {
//            ChatCompletion completion =
//                await client.CompleteChatAsync(messages, options);

//            string jsonText = completion.Content[0].Text;

//            Debug.Log(jsonText);

//            using JsonDocument doc =
//                JsonDocument.Parse(jsonText);

//            string finalAnswer =
//                doc.RootElement
//                .GetProperty("final_answer")
//                .GetString();

//            Debug.Log("[GPT_STU] Final Answer: " + finalAnswer);

//            foreach (JsonElement step in
//                doc.RootElement
//                .GetProperty("steps")
//                .EnumerateArray())
//            {
//                Debug.Log(
//                    "[GPT_STU] Explanation: " +
//                    step.GetProperty("explanation").GetString()
//                );

//                Debug.Log(
//                    "[GPT_STU] Output: " +
//                    step.GetProperty("output").GetString()
//                );
//            }
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError("[GPT_STU] Failed");
//            Debug.LogError(e.Message);
//            Debug.LogError(e.StackTrace);
//        }
//    }
//}