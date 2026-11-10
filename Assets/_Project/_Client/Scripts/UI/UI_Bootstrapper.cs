using UnityEngine;
using UnityEngine.EventSystems; // Обязательно добавляем для работы с EventSystem

public class UI_Bootstrapper : MonoBehaviour
{
    [Header("Префаб Canvas со всеми окнами")]
    [SerializeField] private GameObject canvasPrefab;

    [Header("Префаб EventSystem")]
    [SerializeField] private GameObject eventSystemPrefab;

    private void Awake()
    {
        if (UI_WindowManager.Instance == null)
        {
            if (canvasPrefab != null)
            {
                Instantiate(canvasPrefab);
            }
            else
            {
                Debug.LogError("[UI Bootstrapper] Не назначен canvasPrefab в инспекторе!");
            }
        }

        if (EventSystem.current == null)
        {
            if (eventSystemPrefab != null)
            {
                Instantiate(eventSystemPrefab);
            }
            else
            {
                Debug.LogError("[UI Bootstrapper] Не назначен eventSystemPrefab в инспекторе!");
            }
        }
    }
}