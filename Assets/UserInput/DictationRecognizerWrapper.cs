using UnityEngine;
using UnityEngine.Windows.Speech;
using System;

public class DictationRecognizerWrapper
{
    private DictationRecognizer dictationRecognizer;
    private string recognizedText;

    public bool IsListening { get; private set; }

    public Action<string> OnRecognized;
    public Action<string> OnError;
    public Action OnFinished;

    public DictationRecognizerWrapper()
    {
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationResult += (text, confidence) =>
        {
            recognizedText = text;
            OnRecognized?.Invoke(recognizedText);
        };

        dictationRecognizer.DictationComplete += (completionCause) =>
        {
            IsListening = false;
            if (completionCause == DictationCompletionCause.Complete && !string.IsNullOrEmpty(recognizedText))
            {
                OnFinished?.Invoke();
            }
            else
            {
                OnError?.Invoke("Speech recognition was not completed properly.");
            }
        };

        dictationRecognizer.DictationError += (error, hResult) =>
        {
            IsListening = false;
            OnError?.Invoke($"Error: {error} (HRESULT: {hResult})");
        };
    }

    public void StartListening()
    {
        if (!IsListening)
        {
            dictationRecognizer.Start();
            IsListening = true;
        }
    }

    public void StopListening()
    {
        if (IsListening)
        {
            dictationRecognizer.Stop();
            IsListening = false;
        }
    }

    public void Dispose()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Dispose();
        }
    }
}
