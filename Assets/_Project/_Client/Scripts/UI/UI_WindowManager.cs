using UnityEngine;

public class UI_WindowManager : MonoBehaviour
{

    [Header("Windows")]
    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private GameObject characterWindow;


    private void Awake()
    {
        if (inventoryWindow != null) inventoryWindow.SetActive(false);
        if (characterWindow != null) characterWindow.SetActive(false);
    }

    public void ToggleInventory()
    {
        if (inventoryWindow != null)
        {
            bool currentState = inventoryWindow.activeSelf;
            inventoryWindow.SetActive(!currentState);
        }
    }

    public void ToggleCharacterWindow()
    {
        if (characterWindow != null)
        {
            bool currentState = characterWindow.activeSelf;
            characterWindow.SetActive(!currentState);
        }
    }




}
