using System;
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

        [Header("Item & Skill Spawner Controls")]
        [SerializeField] private TMP_Dropdown itemDropdown;
        [SerializeField] private TMP_InputField itemCountInput;
        [SerializeField] private Button btnSpawnItem;

        [Header("Character Stats Controls (Вкладка игрока)")]
        [SerializeField] private TMP_InputField setLevelInput;
        [SerializeField] private Button btnSetLevel;
        [SerializeField] private Button btnToggleCombat;

        [Header("All Registered Databases")]
        [SerializeField] private List<UniversalItemData> allProjectItems = new List<UniversalItemData>();
        [SerializeField] private List<UniversalSkillData> allProjectSkills = new List<UniversalSkillData>();

        // Внутренний контейнер для маппинга упорядоченных строк дропдауна к реальным ассетам
        private struct CheatListEntry
        {
            public bool isHeader;       // Это заголовок категории (нельзя спавнить)
            public bool isSkill;        // Навык или предмет
            public UniversalItemData itemRef;
            public UniversalSkillData skillRef;
        }

        private readonly List<CheatListEntry> _dropdownMap = new List<CheatListEntry>();

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
            _dropdownMap.Clear();
            List<string> options = new List<string>();

            // 1. Автозагрузка баз данных из Resources
            if (allProjectItems.Count == 0)
            {
                UniversalItemData[] loadedItems = Resources.LoadAll<UniversalItemData>("");
                allProjectItems.AddRange(loadedItems);
            }
            if (allProjectSkills.Count == 0)
            {
                UniversalSkillData[] loadedSkills = Resources.LoadAll<UniversalSkillData>("");
                allProjectSkills.AddRange(loadedSkills);
            }

            // 2. Распределяем предметы по категориям GDD
            List<UniversalItemData> equipment = allProjectItems.FindAll(i => i != null && i.itemType == ItemType.Equipment);
            List<UniversalItemData> capsules = allProjectItems.FindAll(i => i != null && i.itemType == ItemType.Capsule);
            List<UniversalItemData> materials = allProjectItems.FindAll(i => i != null && i.itemType == ItemType.Resource);

            // 3. Распределяем навыки
            List<UniversalSkillData> activeSkills = allProjectSkills.FindAll(s => s != null && !s.isPassive);
            List<UniversalSkillData> passiveSkills = allProjectSkills.FindAll(s => s != null && s.isPassive);

            // --- ФОРМИРУЕМ СТРУКТУРИРОВАННЫЙ СПИСОК ---

            // Категория: Снаряжение
            if (equipment.Count > 0)
            {
                AddCategoryHeader(options, "=== [СНАРЯЖЕНИЕ / ЭКИПИРОВКА] ===");
                foreach (var item in equipment) AddItemItem(options, item);
            }

            // Категория: Капсулы
            if (capsules.Count > 0)
            {
                AddCategoryHeader(options, "=== [КАПСУЛЫ ПРИЗЫВА] ===");
                foreach (var item in capsules) AddItemItem(options, item);
            }

            // Категория: Ресурсы
            if (materials.Count > 0)
            {
                AddCategoryHeader(options, "=== [РЕСУРСЫ КРАФТА] ===");
                foreach (var item in materials) AddItemItem(options, item);
            }

            // Категория: Активные навыки
            if (activeSkills.Count > 0)
            {
                AddCategoryHeader(options, "=== [АКТИВНЫЕ НАВЫКИ ДУШ] ===");
                foreach (var skill in activeSkills) AddSkillItem(options, skill);
            }

            // Категория: Пассивные навыки
            if (passiveSkills.Count > 0)
            {
                AddCategoryHeader(options, "=== [ПАССИВНЫЕ НАВЫКИ ДУШ] ===");
                foreach (var skill in passiveSkills) AddSkillItem(options, skill);
            }

            itemDropdown.AddOptions(options);
        }

        private void AddCategoryHeader(List<string> options, string headerName)
        {
            options.Add(headerName);
            _dropdownMap.Add(new CheatListEntry { isHeader = true });
        }

        private void AddItemItem(List<string> options, UniversalItemData item)
        {
            options.Add($"  • {item.itemName} (ID: {item.itemID})");
            _dropdownMap.Add(new CheatListEntry { isHeader = false, isSkill = false, itemRef = item });
        }

        private void AddSkillItem(List<string> options, UniversalSkillData skill)
        {
            options.Add($"  • [Навык] {skill.skillName} (ID: {skill.skillID})");
            _dropdownMap.Add(new CheatListEntry { isHeader = false, isSkill = true, skillRef = skill });
        }

        private void Cheat_SpawnSelectedItem()
        {
            if (targetPlayer == null || targetPlayer.Inventory == null || _dropdownMap.Count == 0) return;

            int selectedIndex = itemDropdown.value;
            if (selectedIndex < 0 || selectedIndex >= _dropdownMap.Count) return;

            CheatListEntry entry = _dropdownMap[selectedIndex];

            // Защита: Если игрок нажал на заголовок категории
            if (entry.isHeader)
            {
                targetPlayer.SendSystemMessage("Выберите конкретный предмет или навык из списка!");
                return;
            }

            // Считываем количество
            int count = 1;
            if (itemCountInput != null && !string.IsNullOrEmpty(itemCountInput.text))
            {
                int.TryParse(itemCountInput.text, out count);
            }

            // Разделение логики выдачи на основе типа выбранной строки
            if (entry.isSkill)
            {
                // Выдача навыка во 2-ю вкладку инвентаря (используем метод из прошлого шага)
                Debug.Log($"[DevCheat] Выдан навык души во вкладку Навыков: {entry.skillRef.skillName}");
                targetPlayer.Inventory.AddSkill(entry.skillRef);
            }
            else
            {
                // Выдача обычного предмета в 1-ю вкладку инвентаря
                Debug.Log($"[DevCheat] Выдан предмет: {entry.itemRef.itemName} x{count}");
                targetPlayer.Inventory.AddItem(entry.itemRef, count);
            }
        }

        private void Cheat_SetPlayerLevel()
        {
            


        }

        private void Cheat_ToggleCombatState()
        {
            if (targetPlayer == null) return;
            Debug.Log("Переключение боевого режима игрока");
        }
    }
}