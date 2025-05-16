using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json.Linq;

public class TextToSpeechManager : MonoBehaviour
{
    public static TextToSpeechManager Instance;
    private string apiKey;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            LoadApiKey();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadApiKey()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("google-credentials");
        if (jsonFile == null)
        {
            Debug.LogError("Google credentials file not found in Resources folder!");
            return;
        }

        var credentials = JObject.Parse(jsonFile.text);
        apiKey = credentials["api_key"]?.ToString();

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API key not found in google-credentials.json");
        }
    }

    public void Speak(string text)
    {
        StartCoroutine(SendTextToSpeech(text));
    }

    private IEnumerator SendTextToSpeech(string text)
    {
        string jsonBody = @"
        {
          'input':{'text':'" + text + @"'},
          'voice':{
            'languageCode':'ru-RU',
            'name':'ru-RU-Wavenet-C'
          },
          'audioConfig':{
            'audioEncoding':'MP3'
          }
        }";

        string url = $"https://texttospeech.googleapis.com/v1/text:synthesize?key={apiKey}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("TTS Error: " + request.error);
        }
        else
        {
            var jsonResponse = JObject.Parse(request.downloadHandler.text);
            string audioContent = jsonResponse["audioContent"].ToString();
            byte[] audioData = System.Convert.FromBase64String(audioContent);
            PlayAudioClip(audioData);
        }
    }

    private void PlayAudioClip(byte[] audioData)
    {
        var tempFilePath = Path.Combine(Application.persistentDataPath, "ttsAudio.mp3");
        File.WriteAllBytes(tempFilePath, audioData);
        StartCoroutine(LoadAndPlay(tempFilePath));
    }

    private IEnumerator LoadAndPlay(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Audio load error: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
