using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using System;
using System.Collections;
using System.Threading.Tasks;

public class UserInputManager : MonoBehaviour
{
    public static UserInputManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private InputField inputField; // Ссылка на текстовое поле UI для ввода текста
    [SerializeField] private Button voiceInputButton; // Кнопка для активации записи голоса
    [SerializeField] private Button submitButton; // Кнопка для отправки вопроса
    [SerializeField] private Image micIndicator; // Индикатор микрофона (красный или зеленый)

    private bool isListening;
    private DictationRecognizer dictationRecognizer;
    private string recognizedText;

    [SerializeField] private float listeningTimeout = 5f;  // Время ожидания записи голоса

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeDictationRecognizer();
    }

    private void Start()
    {
        voiceInputButton.onClick.AddListener(ToggleVoiceInput);
        submitButton.onClick.AddListener(SubmitQuestion);
    }

    private void InitializeDictationRecognizer()
    {
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            recognizedText = text;
            inputField.text = recognizedText;
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause == DictationCompletionCause.Complete || 
                completionCause == DictationCompletionCause.TimeoutExceeded)
            {
                if (!string.IsNullOrEmpty(recognizedText))
                {
                    ProcessUserInput(recognizedText);
                }
            }
            else
            {
                ShowErrorMessage($"Dictation completed unexpectedly: {completionCause}");
            }

            isListening = false;
            micIndicator.color = Color.red;
        };

        dictationRecognizer.DictationError += (error, hResult) =>
        {
            ShowErrorMessage($"Dictation Error: {error}");
            isListening = false;
            micIndicator.color = Color.red;
        };
    }

    private void ToggleVoiceInput()
    {
        if (isListening)
        {
            StopListening();
        }
        else
        {
            StartListening();
        }
    }

    private void StartListening()
    {
        if (isListening)
            return;

        isListening = true;
        dictationRecognizer.Start();
        micIndicator.color = Color.green;
        Debug.Log("Started voice recognition");

        inputField.text = "";
        StartCoroutine(StopListeningAfterTimeout());
    }

    private void StopListening()
    {
        if (!isListening)
            return;

        dictationRecognizer.Stop();
        micIndicator.color = Color.red;
        Debug.Log("Stopped voice recognition");
        isListening = false;
    }

    private IEnumerator StopListeningAfterTimeout()
    {
        yield return new WaitForSeconds(listeningTimeout);
        if (isListening)
        {
            StopListening();
        }
    }

    private void ProcessUserInput(string input)
    {
        Debug.Log($"User question: {input}");

        GPTQuestionAnswerer.Instance.GetAnswer(input, answer =>
        {
            Speak(answer);
        });
    }

    private void SubmitQuestion()
    {
        string userQuestion = inputField.text;

        if (!string.IsNullOrEmpty(userQuestion))
        {
            ProcessUserInput(userQuestion);
        }
        else
        {
            Debug.LogWarning("No question entered.");
        }
    }

    private void Speak(string text)
    {
        TextToSpeechManager.Instance.Speak(text);
    }

    private void ShowErrorMessage(string message)
    {
        Debug.LogError(message);
    }

    private void OnApplicationQuit()
    {
        if (dictationRecognizer != null && isListening)
        {
            dictationRecognizer.Stop();
        }
    }
}
