using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using System;

public class GPTAttractionSelector : MonoBehaviour
{
    public static GPTAttractionSelector Instance { get; private set; }

    [Header("OpenAI API Settings")]
    [SerializeField] private string openAIKey = "sk-..."; // 🔒 Укажи свой API ключ
    [SerializeField] private string model = "gpt-3.5-turbo";

    private List<Attraction> allAttractions;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        allAttractions = AttractionDataManager.Instance.GetAttractions();
    }

    public void SelectAttractions(string userInterests, Action<List<Attraction>> onResult)
    {
        if (allAttractions == null || allAttractions.Count == 0)
        {
            Debug.LogWarning("Список достопримечательностей пуст.");
            onResult?.Invoke(new List<Attraction>());
            return;
        }

        string attractionsListJson = JsonConvert.SerializeObject(allAttractions);
        string systemPrompt = "Ты гид. Тебе дан список достопримечательностей в формате JSON и интересы пользователя. Выбери подходящие достопримечательности по интересам и верни их названия в JSON-массиве, например: [\"Эрмитаж\", \"Космодром\"]";

        string userPrompt = $"Интересы пользователя: {userInterests}\nВот список достопримечательностей:\n{attractionsListJson}";

        //StartCoroutine(SendToGPT(systemPrompt, userPrompt, onResult));
    }

//  IEnumerator SendToGPT(string systemMessage, string userMessage, Action<List<Attraction>> onResult)
// {
//     var requestData = new
//     {
//         model = model,
//         messages = new[]
//         {
//             new { role = "system", content = systemMessage },
//             new { role = "user", content = userMessage }
//         }
//     };

//     string jsonData = JsonConvert.SerializeObject(requestData);

//     using (UnityWebRequest request = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
//     {
//         byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
//         request.uploadHandler = new UploadHandlerRaw(bodyRaw);
//         request.downloadHandler = new DownloadHandlerBuffer();
//         request.SetRequestHeader("Content-Type", "application/json");
//         request.SetRequestHeader("Authorization", $"Bearer {openAIKey}");

//         yield return request.SendWebRequest();

//         if (request.result != UnityWebRequest.Result.Success)
//         {
//             Debug.LogError($"GPT Error: {request.error}");
//             onResult?.Invoke(new List<Attraction>());
//             yield break;
//         }

//         string responseText = request.downloadHandler.text;
//         try
//         {
//             var response = JsonConvert.DeserializeObject<OpenAIResponse>(responseText);
//             string content = response.choices[0].message.content;

//             // Ожидается JSON массив названий
//             List<string> names = JsonConvert.DeserializeObject<List<string>>(content);

//             List<Attraction> result = allAttractions
//                 .Where(attr => names.Any(name => attr.attractionName.ToLower().Contains(name.ToLower())))
//                 .ToList();

//             onResult?.Invoke(result);

//             // 🗣 Озвучка результата
//             if (result.Count > 0)
//             {
//                 string joinedNames = string.Join(", ", result.Select(attr => attr.attractionName));
//                 string speechText = $"По вашим интересам рекомендую посетить: {joinedNames}";
//                 TextToSpeechManager.Instance.Speak(speechText);
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"Ошибка парсинга GPT: {ex.Message}");
//             onResult?.Invoke(new List<Attraction>());
//         }
//     }
// }


    // Вспомогательные классы для десериализации ответа от OpenAI
    [System.Serializable]
    private class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }
}
