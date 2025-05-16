using UnityEngine;
using System.Collections.Generic;
using System.IO;

    [System.Serializable]
    public class Position
    {
        public float x, y, z;
    }

    [System.Serializable]
    public class Attraction
    {
        public string attractionName;
        public string description;
        public Position position;
    }

    [System.Serializable]
    public class AttractionList
    {
        public List<Attraction> attractions;
    }
public class AttractionDataManager : MonoBehaviour
{

    public static AttractionDataManager Instance { get; private set; }
    public string jsonFilePath = "Assets/Resources/attractions.json";
    public AttractionList loadedAttractions;

    void Start()
    {
        LoadAttractionsFromJson();
    }

    public void LoadAttractionsFromJson()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"JSON file not found at path: {jsonFilePath}");
            return;
        }

        string json = File.ReadAllText(jsonFilePath);
        loadedAttractions = JsonUtility.FromJson<AttractionList>(json);

        if (loadedAttractions == null || loadedAttractions.attractions.Count == 0)
        {
            Debug.LogError("No attractions found in JSON or failed to parse.");
        }
    }

    public List<Attraction> GetAttractions()
    {
        return loadedAttractions?.attractions ?? new List<Attraction>();
    }
}
