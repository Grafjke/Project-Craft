using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using ProjectCraft._Shared;
using ProjectCraft._Client.UI;

public class UI_WindowManager : MonoBehaviour
{
    public static UI_WindowManager Instance { get; private set; }

    [Header("Right Side Window")]
    [SerializeField] private GameObject inventoryWindow;

    [Header("Left Side Windows Group")]
    [SerializeField] private GameObject[] leftSideWindows;

    [Header("Specific Left Windows References")]
    [SerializeField] private GameObject _characterWindow;
    [SerializeField] private GameObject _altarWindow;
    [SerializeField] private GameObject _craftWindow;

    public GameObject characterWindow => _characterWindow;
    public GameObject altarWindow => _altarWindow;
    public GameObject craftWindow => _craftWindow;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (inventoryWindow != null) inventoryWindow.SetActive(false);
        CloseAllLeftWindows();

        OptimizeUIForCurrentScene();
    }

    private void OptimizeUIForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName != "Citadel_Prototype")
        {
            Debug.Log($"[UI Optimizer] Локация '{currentSceneName}' — боевая. Уничтожаем окна Цитадели.");
            if (_altarWindow != null) Destroy(_altarWindow);
            if (_craftWindow != null) Destroy(_craftWindow);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (_characterWindow != null) ToggleLeftWindowExclusive(_characterWindow);
        }
    }

    public GameObject GetWindowByType(UIWindowType type)
    {
        return type switch
        {
            UIWindowType.Character => _characterWindow,
            UIWindowType.Craft => _craftWindow,
            UIWindowType.Altar => _altarWindow,
            _ => null
        };
    }

    public void OpenWindowForNPC(GameObject targetWindow, MonoBehaviour npcSource)
    {
        if (targetWindow == null)
        {
            Debug.LogWarning($"Окно для {npcSource.name} недоступно на этой сцене!");
            return;
        }

        ToggleLeftWindowExclusive(targetWindow);

        if (targetWindow.activeInHierarchy)
        {
            var interactableWindow = targetWindow.GetComponent<IInteractableUIWindow>();
            if (interactableWindow != null)
            {
                interactableWindow.OnUIWindowOpened(npcSource);
            }
        }
    }

    public void ToggleInventory()
    {
        if (inventoryWindow == null) return;
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
    }

    public void ToggleLeftWindowExclusive(GameObject targetWindow)
    {
        if (targetWindow == null) return;

        bool wasActive = targetWindow.activeSelf;

        if (leftSideWindows != null)
        {
            for (int i = 0; i < leftSideWindows.Length; i++)
            {
                if (leftSideWindows[i] != null)
                {
                    leftSideWindows[i].SetActive(false);
                }
            }
        }
        targetWindow.SetActive(!wasActive);
    }

    public void CloseAllLeftWindows()
    {
        if (leftSideWindows == null) return;

        for (int i = 0; i < leftSideWindows.Length; i++)
        {
            if (leftSideWindows[i] != null)
            {
                leftSideWindows[i].SetActive(false);
            }
        }
    }
}