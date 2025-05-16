using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class GPTQuestionAnswerer : MonoBehaviour
{
    public static GPTQuestionAnswerer Instance { get; private set; }

    [SerializeField] private string apiKey; // Ваш ключ API для доступа к GPT
    [SerializeField] private string model = "text-davinci-003"; // Выбор модели
    [SerializeField] private int maxTokens = 150; // Ограничение на количество токенов

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Метод для получения ответа от GPT
    public void GetAnswer(string question, Action<string> onAnswerReceived)
    {
        if (string.IsNullOrEmpty(question))
        {
            Debug.LogWarning("Received empty question.");
            onAnswerReceived?.Invoke("Пожалуйста, задайте вопрос.");
            return;
        }

        ConversationLogger.Instance.LogUserQuestion(question);
        StartCoroutine(FetchAnswerFromGPT(question, onAnswerReceived));
    }

    private IEnumerator FetchAnswerFromGPT(string question, Action<string> onAnswerReceived)
    {
        string url = "https://api.openai.com/v1/completions";
        string requestBody = "{\"model\": \"" + model + "\", \"prompt\": \"" + question + "\", \"max_tokens\": " + maxTokens + "}";

        using (var www = new UnityWebRequest(url, "POST"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Ждем ответа от сервера
            yield return www.SendWebRequest();

            // Проверка на ошибки сети
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Если ответ успешен, парсим его
                string response = www.downloadHandler.text;
                string answer = ParseGPTResponse(response);
                onAnswerReceived?.Invoke(answer);
                ConversationLogger.Instance.LogBotAnswer(answer);
                if (!string.IsNullOrEmpty(answer))
                {
                    TextToSpeechManager.Instance.Speak(answer);
                }
            }
            else
            {
                // Ошибка сети или сервера
                Debug.LogError("Error fetching answer: " + www.error);
                ConversationLogger.Instance.LogError(www.error);
                onAnswerReceived?.Invoke("Извините, произошла ошибка при получении ответа.");
            }
        }
    }

    private string ParseGPTResponse(string response)
    {
        // Простой парсинг JSON-ответа
        try
        {
            var jsonResponse = SimpleJSON.JSON.Parse(response);
            return jsonResponse["choices"][0]["text"].Value.Trim();
        }
        catch (Exception e)
        {
            Debug.LogError("Error parsing GPT response: " + e.Message);
            return "Извините, я не понял ответа от сервера.";
        }
    }
}
