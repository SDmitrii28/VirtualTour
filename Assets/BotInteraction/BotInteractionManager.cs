using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BotInteractionManager : MonoBehaviour
{
    public static BotInteractionManager Instance { get; private set; }

    private List<Attraction> filteredAttractions;
    private int currentAttractionIndex;
    private bool isWaitingForQuestion;

    [SerializeField] private Transform botTransform;
    [SerializeField] private float moveSpeed = 2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        AskUserForInterests();
    }

    private void AskUserForInterests()
    {
        string greeting = "Привет! Какие у вас интересы в путешествии? История, искусство, технологии или что-то другое?";
        Speak(greeting);

        UIManager.Instance.UpdateUIState("waiting_for_interests");

        ConversationLogger.Instance.LogMessage($"Bot asks: {greeting}");

        UserInputManager.Instance.ListenForInput(OnUserProvidedInterests);
    }

    public void OnUserProvidedInterests(string interests)
    {
        ConversationLogger.Instance.LogUserQuestion($"User interests: {interests}");

        GPTAttractionSelector.Instance.SelectAttractions(interests, result =>
        {
            filteredAttractions = result;

            if (filteredAttractions == null || filteredAttractions.Count == 0)
        {
            string errorMessage = "Извините, я не нашёл подходящих достопримечательностей. Попробуйте ещё раз.";
            UIManager.Instance.UpdateUIState("waiting_for_error");
            Speak(errorMessage);
            ConversationLogger.Instance.LogError(errorMessage);
            AskUserForInterests();
            return;
    }

    currentAttractionIndex = 0;
    NavigateToCurrentAttraction();
});

    }

    private void NavigateToCurrentAttraction()
    {
        var attraction = filteredAttractions[currentAttractionIndex];
        Vector3 targetPos = new Vector3(attraction.position.x, botTransform.position.y, attraction.position.y);
        Navigation.GPSNavigationManager.Instance.StartNavigation(new List<Vector3> { targetPos });
        AnimationManager.Instance.PlayWalkingAnimation();
        ConversationLogger.Instance.LogMessage($"Bot is navigating to: {attraction.attractionName} at position {targetPos}");
    }

    public void OnAttractionReached()
    {
        var attraction = filteredAttractions[currentAttractionIndex];

        AnimationManager.Instance.PlayTalkingAnimation();

        ConversationLogger.Instance.LogMessage($"Bot reached attraction: {attraction.attractionName}");

        Speak(attraction.description, () =>
        {
            AnimationManager.Instance.PlayIdleAnimation();
            PromptUserQuestions(attraction);
        });
    }

    private void PromptUserQuestions(Attraction attraction)
    {
        isWaitingForQuestion = true;
        string promptMessage = "У вас есть вопросы по этой достопримечательности?";
        Speak(promptMessage);
        UIManager.Instance.UpdateUIState("waiting_for_question");

        ConversationLogger.Instance.LogMessage($"Bot asks: {promptMessage}");

        UserInputManager.Instance.ListenForInput(userQuestion =>
        {
            ConversationLogger.Instance.LogUserQuestion($"User question: {userQuestion}");

            if (string.IsNullOrWhiteSpace(userQuestion) || userQuestion.ToLower().Contains("нет"))
            {
                isWaitingForQuestion = false;
                GoToNextAttraction();
            }
            else
            {
                string prompt = $"{attraction.description}\nВопрос: {userQuestion}";
                GPTQuestionAnswerer.Instance.GetAnswer(prompt, answer =>
                {
                    ConversationLogger.Instance.LogBotAnswer($"Bot answer: {answer}");

                    Speak(answer, () => PromptUserQuestions(attraction));
                });
            }
        });
    }

    private void GoToNextAttraction()
    {
        currentAttractionIndex++;
        if (currentAttractionIndex >= filteredAttractions.Count)
        {
            string endMessage = "Экскурсия завершена. Спасибо за внимание!";
            AnimationManager.Instance.PlayIdleAnimation();
            Speak(endMessage);
            UIManager.Instance.UpdateUIState("end_of_tour");

            ConversationLogger.Instance.LogMessage(endMessage);
        }
        else
        {
            NavigateToCurrentAttraction();
        }
    }

    private void Speak(string text, System.Action onComplete = null)
    {
        TextToSpeechManager.Instance.Speak(text, onComplete);
    }
}
