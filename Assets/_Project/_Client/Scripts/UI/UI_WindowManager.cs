using UnityEngine;
using UnityEngine.InputSystem;

public class UI_WindowManager : MonoBehaviour
{
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
        if (inventoryWindow != null) inventoryWindow.SetActive(false);
        CloseAllLeftWindows();
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
            ToggleLeftWindowExclusive(_characterWindow);
        }
    }

    public void ToggleInventory()
    {
        if (inventoryWindow == null) return;
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
    }

    public void ToggleLeftWindowExclusive(GameObject targetWindow)
    {
        if (targetWindow == null)
        {
            return;
        }

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