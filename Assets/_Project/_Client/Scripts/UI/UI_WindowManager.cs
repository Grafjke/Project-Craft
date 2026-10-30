using UnityEngine;
using UnityEngine.InputSystem;

public class UI_WindowManager : MonoBehaviour
{
    [Header("Right Side Window")]
    [SerializeField] private GameObject inventoryWindow;

    [Header("Left Side Windows Group")]
    [SerializeField] private GameObject[] leftSideWindows;


    [Header("Specific Left Windows References")]
    [SerializeField] private GameObject characterWindow;

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
            ToggleLeftWindowExclusive(characterWindow);
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

        CloseAllLeftWindows();

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
