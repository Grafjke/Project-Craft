using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectCraft._Server;
using ProjectCraft._Shared;

namespace ProjectCraft._Client.UI
{
    public class UI_DevCheatConsole : MonoBehaviour
    {
        [Header("Global References")]
        [SerializeField] private ServerPlayerEntity targetPlayer;
        [SerializeField] private GameObject consolePanel;

        [Header("Item Spawner Controls (Вкладка предметов)")]
        [SerializeField] private TMP_Dropdown itemDropdown;  
        [SerializeField] private TMP_InputField itemCountInput;
        [SerializeField] private Button btnSpawnItem;

        [Header("Character Stats Controls (Вкладка игрока)")]
        [SerializeField] private TMP_InputField setLevelInput;
        [SerializeField] private Button btnSetLevel;
        [SerializeField] private Button btnToggleCombat;

        [Header("All Registered Items Database")]
        [SerializeField] private List<UniversalItemData> allProjectItems = new List<UniversalItemData>();

        private void Start()
        {
            if (targetPlayer == null)
                targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();

           
            if (consolePanel != null) consolePanel.SetActive(false);

            PopulateItemDropdown();

            if (btnSpawnItem != null) btnSpawnItem.onClick.AddListener(Cheat_SpawnSelectedItem);
            if (btnSetLevel != null) btnSetLevel.onClick.AddListener(Cheat_SetPlayerLevel);
            if (btnToggleCombat != null) btnToggleCombat.onClick.AddListener(Cheat_ToggleCombatState);
        }

        private void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                if (consolePanel != null)
                {
                    consolePanel.SetActive(!consolePanel.activeSelf);
                }
            }
        }

        private void PopulateItemDropdown()
        {
            if (itemDropdown == null) return;

            itemDropdown.ClearOptions();
            List<string> options = new List<string>();

            if (allProjectItems.Count == 0)
            {
                UniversalItemData[] loadedItems = Resources.LoadAll<UniversalItemData>("");
                allProjectItems.AddRange(loadedItems);
            }

            foreach (var item in allProjectItems)
            {
                if (item != null)
                {
                    options.Add($"{item.itemName} (ID: {item.itemID})");
                }
            }

            itemDropdown.AddOptions(options);
        }


        private void Cheat_SpawnSelectedItem()
        {
            if (targetPlayer == null || targetPlayer.Inventory == null || allProjectItems.Count == 0) return;

            int selectedIndex = itemDropdown.value;
            if (selectedIndex < 0 || selectedIndex >= allProjectItems.Count) return;

            UniversalItemData selectedItem = allProjectItems[selectedIndex];

            int count = 1;
            if (itemCountInput != null && !string.IsNullOrEmpty(itemCountInput.text))
            {
                int.TryParse(itemCountInput.text, out count);
            }

            Debug.Log($"Выдан предмет: {selectedItem.itemName} x{count}");
            targetPlayer.Inventory.AddItem(selectedItem, count);
        }

        private void Cheat_SetPlayerLevel()
        {
            if (targetPlayer == null || setLevelInput == null) return;

            if (int.TryParse(setLevelInput.text, out int targetLevel))
            {
                // ЗАГЛУШКА!!!!!!!!!

                Debug.Log($"Запрос на изменение уровня на: {targetLevel}");

            }
        }

        private void Cheat_ToggleCombatState()
        {
            // ЗАГЛУШКА!!!!!!!!!

            if (targetPlayer == null) return;
            Debug.Log("Переключение боевого режима игрока");
        }
    }
}