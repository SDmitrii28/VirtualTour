using System;
using System.IO;
using UnityEngine;

public class ConversationLogger : MonoBehaviour
{
    private static string logFilePath = "conversation_log.txt";  // Путь к файлу для записи логов

    public static ConversationLogger Instance { get; private set; }


private void Awake()
{
    if (Instance == null)
        Instance = this;
    else
    {
        Destroy(gameObject);
        return;
    }

    logFilePath = Path.Combine(Application.persistentDataPath, "conversation_log.txt");
}


    // Метод для записи сообщения в лог
    public void LogMessage(string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timestamp}: {message}\n";

        // Печатаем в консоль для отладки
        Debug.Log(logEntry);

        // Записываем в файл
        File.AppendAllText(logFilePath, logEntry);
    }

    // Логирование вопроса пользователя
    public void LogUserQuestion(string question)
    {
        LogMessage($"User Question: {question}");
    }

    // Логирование ответа бота
    public void LogBotAnswer(string answer)
    {
        LogMessage($"Bot Answer: {answer}");
    }

    // Логирование ошибок
    public void LogError(string errorMessage)
    {
        LogMessage($"ERROR: {errorMessage}");
    }

    // Очистка логов
    public void ClearLogs()
    {
        File.WriteAllText(logFilePath, string.Empty);  // Очищаем файл
        Debug.Log("Conversation log cleared.");
    }

    // Метод для отображения содержимого лога (для отладки)
    public void DisplayLogs()
    {
        if (File.Exists(logFilePath))
        {
            string logContents = File.ReadAllText(logFilePath);
            Debug.Log(logContents);  // Выводим содержимое в консоль
        }
        else
        {
            Debug.Log("Log file does not exist.");
        }
    }
}
