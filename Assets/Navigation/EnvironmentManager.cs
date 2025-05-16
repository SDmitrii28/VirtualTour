using UnityEngine;
using System.Collections.Generic;

namespace Navigation
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance;

        [Header("Attraction Prefabs")]
        public List<AttractionPrefab> attractionPrefabs;  // Список префабов для различных достопримечательностей

        [Header("Parent")]
        public Transform attractionsParent;

        private List<GameObject> activeAttractions = new List<GameObject>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        // Загружаем достопримечательности на сцену с учетом их уникальных префабов
        public void LoadAttractionsToScene(List<Attraction> attractions)
        {
            ClearPreviousAttractions();

            foreach (var attr in attractions)
            {
                // Ищем подходящий префаб для достопримечательности
                GameObject attractionPrefab = GetAttractionPrefab(attr.attractionName);
                if (attractionPrefab != null)
                {
                    Vector3 pos = new Vector3(attr.position.x, attr.position.y, attr.position.z);
                    GameObject obj = Instantiate(attractionPrefab, pos, Quaternion.identity, attractionsParent);
                    obj.name = attr.attractionName;
                    // Здесь можно установить надписи, иконки и т.д.
                    activeAttractions.Add(obj);
                }
                else
                {
                    Debug.LogWarning("Не найден префаб для достопримечательности: " + attr.attractionName);
                }
            }
        }

        // Обновление состояния аттракциона (посещено/не посещено)
        public void UpdateAttractionState(string attractionName, AttractionState state)
        {
            GameObject attraction = activeAttractions.Find(obj => obj.name == attractionName);
            if (attraction != null)
            {
                var renderer = attraction.GetComponent<Renderer>();
                if (renderer != null)
                {
                    switch (state)
                    {
                        case AttractionState.Visited:
                            renderer.material.color = Color.green;
                            break;
                        case AttractionState.NotVisited:
                            renderer.material.color = Color.red;
                            break;
                        case AttractionState.InProgress:
                            renderer.material.color = Color.yellow;
                            break;
                    }
                }
            }
        }

        // Обновление иконки и текста аттракциона
        public void UpdateAttractionIconAndText(string attractionName, string iconName, string description)
        {
            GameObject attraction = activeAttractions.Find(obj => obj.name == attractionName);
            if (attraction != null)
            {
                // Изменяем иконку
                var icon = attraction.GetComponentInChildren<SpriteRenderer>();
                if (icon != null)
                {
                    icon.sprite = Resources.Load<Sprite>(iconName);
                }

                // Обновляем описание
                var textComponent = attraction.GetComponentInChildren<UnityEngine.UI.Text>();
                if (textComponent != null)
                {
                    textComponent.text = description;
                }
            }
        }

        // Получаем префаб для данной достопримечательности по имени
        private GameObject GetAttractionPrefab(string attractionName)
        {
            foreach (var item in attractionPrefabs)
            {
                if (item.attractionName == attractionName)
                {
                    return item.prefab;
                }
            }

            return null;  // Если префаб не найден
        }

        // Очистка предыдущих достопримечательностей
        private void ClearPreviousAttractions()
        {
            foreach (var obj in activeAttractions)
            {
                if (obj != null)
                {
                    obj.SetActive(false);  // Отключаем, а не уничтожаем
                }
            }

            activeAttractions.Clear();
        }
    }

    // Перечисление для разных состояний аттракциона
    public enum AttractionState
    {
        NotVisited,
        Visited,
        InProgress
    }

    // Структура для связывания имени достопримечательности с её префабом
    [System.Serializable]
    public class AttractionPrefab
    {
        public string attractionName;
        public GameObject prefab;
    }

    // Пример структуры Attraction (для демонстрации)
    [System.Serializable]
    public class Attraction
    {
        public string attractionName;
        public Vector3 position;
    }
}
