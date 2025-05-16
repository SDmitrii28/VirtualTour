using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Text botMessageText; // Текстовое поле для сообщений от бота
    [SerializeField] private Button confirmButton; // Кнопка подтверждения
    [SerializeField] private Button skipButton; // Кнопка пропуска
    [SerializeField] private GameObject modalWindow; // Модальное окно
    [SerializeField] private Text modalText; // Текст в модальном окне
    [SerializeField] private GameObject questionInputPanel; // Панель ввода вопроса
    [SerializeField] private InputField questionInputField; // Поле ввода вопроса
    [SerializeField] private Button sendButton; // Кнопка отправки вопроса
    [SerializeField] private Button micButton; // Кнопка для включения/выключения микрофона
    [SerializeField] private Text micButtonText; // Текст на кнопке микрофона

    private bool isMicOn = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Настройка кнопок
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
        sendButton.onClick.AddListener(OnSendButtonClicked);
        micButton.onClick.AddListener(ToggleMic);

        // Изначально скрываем панель ввода вопроса и кнопки
        questionInputPanel.SetActive(false);
        SetButtonsActive(false, false);
        UpdateMicButtonText();
        
        // Изначально скрываем модальное окно
        modalWindow.SetActive(false);
    }

    public void ShowBotMessage(string message)
    {
        botMessageText.text = message;
        botMessageText.gameObject.SetActive(true); // Показываем текст
    }

    public void HideBotMessage()
    {
        botMessageText.gameObject.SetActive(false); // Скрываем текст
    }

    // Включаем/выключаем кнопки в зависимости от контекста
    public void SetButtonsActive(bool confirmActive, bool skipActive)
    {
        confirmButton.gameObject.SetActive(confirmActive);
        skipButton.gameObject.SetActive(skipActive);
    }

    // Показать панель ввода вопроса
    public void ShowQuestionInputPanel()
    {
        questionInputPanel.SetActive(true);
        questionInputField.text = ""; // Очистка поля ввода
        sendButton.gameObject.SetActive(true); // Показываем кнопку отправки
        micButton.gameObject.SetActive(true); // Показываем кнопку микрофона
    }

    // Скрыть панель ввода вопроса
    public void HideQuestionInputPanel()
    {
        questionInputPanel.SetActive(false);
        sendButton.gameObject.SetActive(false);
        micButton.gameObject.SetActive(false);
    }

    // Включить или выключить микрофон
    private void ToggleMic()
    {
        isMicOn = !isMicOn;
        UpdateMicButtonText();

        if (isMicOn)
        {
            // Логика для начала прослушивания
            Debug.Log("Микрофон включен. Готов к приему голосовых команд.");
            // Здесь можно подключить компонент распознавания речи, если используется
        }
        else
        {
            // Логика для остановки прослушивания
            Debug.Log("Микрофон выключен.");
            // Остановить прослушивание
        }
    }

    // Обновить текст на кнопке микрофона в зависимости от состояния
    private void UpdateMicButtonText()
    {
        micButtonText.text = isMicOn ? "Выключить микрофон" : "Включить микрофон";
    }

    // Обработчик кнопки отправки вопроса
    private void OnSendButtonClicked()
    {
        string userQuestion = questionInputField.text;
        if (string.IsNullOrWhiteSpace(userQuestion))
        {
            return;
        }

        Debug.Log($"Отправлен вопрос: {userQuestion}");

        // Логика для отправки вопроса и получения ответа от бота
        BotInteractionManager.Instance.OnUserProvidedInterest(userQuestion);

        // Скрыть панель ввода после отправки
        HideQuestionInputPanel();
    }

    // Обработчик кнопки подтверждения
    private void OnConfirmButtonClicked()
    {
        // Логика для подтверждения действия, например:
        Debug.Log("Confirm button clicked.");
    }

    // Обработчик кнопки пропуска
    private void OnSkipButtonClicked()
    {
        // Логика для пропуска текущего шага
        Debug.Log("Skip button clicked.");
    }

    // Показать модальное окно с текстом
    public void ShowModal(string message)
    {
        modalText.text = message;
        modalWindow.SetActive(true);
    }

    // Скрыть модальное окно
    public void HideModal()
    {
        modalWindow.SetActive(false);
    }

    // Метод для обновления UI в зависимости от состояния
    public void UpdateUIState(string state)
    {
        switch (state)
        {
            case "greeting":
                ShowBotMessage("Привет! Добро пожаловать на экскурсию.");
                SetButtonsActive(true, false);
                break;
            case "waiting_for_interests":
                ShowBotMessage("Привет! Какие у вас интересы в путешествии? История, искусство, технологии или что-то другое?");
                SetButtonsActive(true, false);
                break;
            case "waiting_for_question":
                ShowBotMessage("У вас есть вопросы по этой достопримечательности?");
                ShowQuestionInputPanel();
                break;
            case "waiting_for_error":
                ShowBotMessage("Извините, я не нашёл подходящих достопримечательностей. Попробуйте ещё раз.");
                ShowQuestionInputPanel();
                break;
            case "end_of_tour":
                ShowModal("Экскурсия завершена! Спасибо за внимание.");
                break;
            default:
                Debug.LogWarning("Unknown state: " + state);
                break;
        }
    }
}
