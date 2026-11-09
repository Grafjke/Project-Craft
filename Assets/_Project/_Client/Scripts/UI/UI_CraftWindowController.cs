using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectCraft._Server;
using ProjectCraft._Shared;

namespace ProjectCraft._Client.UI
{
    public class UI_CraftWindowController : MonoBehaviour
    {
        [Header("Systems Link")]
        [SerializeField] private ServerCraftSystem serverCraftSystem;

        [Header("Global References")]
        [SerializeField] private ServerPlayerEntity targetPlayer;

        [Header("Panel 2 (Items List)")]
        [SerializeField] private Transform itemsGridParent;
        [SerializeField] private GameObject itemButtonPrefab;

        [Header("Panel 3 (Recipe Inspector)")]
        [SerializeField] private Image selectedItemIcon;
        [SerializeField] private TextMeshProUGUI selectedItemDescription;
        [SerializeField] private Transform materialsGridParent;
        [SerializeField] private GameObject materialSlotPrefab;
        [SerializeField] private Button btnCreate;

        [Header("Cascade Panels References")]
        [SerializeField] private GameObject panel1Categories;
        [SerializeField] private GameObject panel2ItemsList;
        [SerializeField] private GameObject panel3RecipeInspector;

        private ServerNPCBlacksmith _currentBlacksmithContext;


        private Dictionary<string, List<UniversalItemData>> _recipesByCategory = new Dictionary<string, List<UniversalItemData>>();
        private List<GameObject> _spawnedItemButtons = new List<GameObject>();
        private List<GameObject> _spawnedMaterialSlots = new List<GameObject>();

        private UniversalItemData _currentlySelectedItem;

        public void InitializeInteraction(ServerNPCBlacksmith blacksmith)
        {
            _currentBlacksmithContext = blacksmith;
        }

        private void Awake()
        {
            LoadRecipesFromProject();

            if (btnCreate != null) btnCreate.onClick.AddListener(RequestCraft);
        }
        private void Update()
        {
            // Если контекста кузнеца нет, ничего не считаем
            if (_currentBlacksmithContext == null) return;

            // Если игрока нет, пробуем найти его на сцене динамически
            if (targetPlayer == null)
            {
                targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();
            }

            if (targetPlayer != null)
            {
                float distance = Vector3.Distance(targetPlayer.transform.position, _currentBlacksmithContext.transform.position);

                // Если игрок отошел дальше радиуса взаимодействия кузнеца + небольшой запас
                if (distance > (_currentBlacksmithContext.InteractionRadius + 0.5f))
                {
                    Debug.Log($"[CraftUI] Игрок отошел слишком далеко ({distance:F1}м). Автоматическое закрытие окна крафта.");

                    var windowManager = FindFirstObjectByType<UI_WindowManager>();
                    if (windowManager != null)
                    {
                        windowManager.CloseAllLeftWindows();
                    }

                    _currentBlacksmithContext = null;
                }
            }
        }

        private void OnEnable()
        {
            if (targetPlayer == null) targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();

            ResetToFirstCascadeStep();
        }

        private void OnDisable()
        {
            _currentBlacksmithContext = null;
        }

        private void LoadRecipesFromProject()
        {
            _recipesByCategory.Clear();

            UniversalItemData[] allEquipment = Resources.LoadAll<UniversalItemData>("ScriptableObjects/Items/Equipments");

            foreach (var item in allEquipment)
            {
                if (item == null) continue;

                string category = item.weaponOrArmorTag;

                if (!_recipesByCategory.ContainsKey(category))
                {
                    _recipesByCategory[category] = new List<UniversalItemData>();
                }
                _recipesByCategory[category].Add(item);
            }
        }

        public void SelectCategory(string categoryName)
        {
            ClearItemButtons();
            ClearInspector();

            if (!_recipesByCategory.ContainsKey(categoryName)) return;

            if (panel2ItemsList != null) panel2ItemsList.SetActive(true);
            if (panel3RecipeInspector != null) panel3RecipeInspector.SetActive(false);

            foreach (var recipe in _recipesByCategory[categoryName])
            {
                GameObject btnGo = Instantiate(itemButtonPrefab, itemsGridParent);

                UI_CraftItemButton craftButton = btnGo.GetComponent<UI_CraftItemButton>();
                if (craftButton != null)
                {
                    craftButton.SetupButton(recipe, () => SelectItemForCrafting(recipe));
                }
                else
                {
                    Debug.LogError($"[CraftUI] На префабе {itemButtonPrefab.name} отсутствует компонент UI_CraftItemButton!");
                }

                _spawnedItemButtons.Add(btnGo);
            }

            if (itemsGridParent != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(itemsGridParent.GetComponent<RectTransform>());
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        public void SelectItemForCrafting(UniversalItemData item)
        {
            ClearMaterialSlots();
            _currentlySelectedItem = item;

            if (panel3RecipeInspector != null) panel3RecipeInspector.SetActive(true);

            if (selectedItemIcon != null)
            {
                selectedItemIcon.enabled = true;
                selectedItemIcon.sprite = item.itemIcon;
            }

            if (selectedItemDescription != null)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine($"<size=120%><b>{item.itemName}</b></size>");
                sb.AppendLine($"Требуемый уровень: {item.requiredLevel}");
                sb.AppendLine("\n<b>Характеристики:</b>");

                if (item.staticStatsBlueprint != null)
                {
                    foreach (var stat in item.staticStatsBlueprint)
                    {
                        sb.AppendLine($"• {stat.statName}: +{stat.baseValue}");
                    }
                }
                selectedItemDescription.text = sb.ToString();
            }

            bool canCraft = true;

            if (item.requiredMaterials != null)
            {
                foreach (var reqMaterial in item.requiredMaterials)
                {
                    GameObject matSlotGo = Instantiate(materialSlotPrefab, materialsGridParent);
                    _spawnedMaterialSlots.Add(matSlotGo);

                    UI_CraftMaterialSlot materialSlot = matSlotGo.GetComponent<UI_CraftMaterialSlot>();
                    if (materialSlot != null)
                    {
                        materialSlot.SetupMaterialSlot(reqMaterial, targetPlayer);
                    }

                    if (targetPlayer != null && targetPlayer.Inventory != null)
                    {
                        int currentInInventory = targetPlayer.Inventory.GetItemCount(reqMaterial.resource);
                        if (currentInInventory < reqMaterial.count)
                        {
                            canCraft = false;
                        }
                    }
                    else
                    {
                        canCraft = false;
                    }
                }
                if (materialsGridParent != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(materialsGridParent.GetComponent<RectTransform>());
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            }

            if (btnCreate != null)
            {
                bool hasLevel = targetPlayer != null && targetPlayer.CharacterLevel >= item.requiredLevel;
                btnCreate.interactable = canCraft && hasLevel;
            }
        }

        private void RequestCraft()
        {
            if (_currentlySelectedItem == null || targetPlayer == null) return;

            if (btnCreate != null) btnCreate.interactable = false;

            if (serverCraftSystem != null)
            {
                Debug.Log($"Отправка запроса на создание предмета: {_currentlySelectedItem.itemName}");

                serverCraftSystem.CraftItem(targetPlayer, _currentlySelectedItem);

                SelectItemForCrafting(_currentlySelectedItem);
            }
            else
            {
                Debug.LogError("Ссылка на ServerCraftSystem не установлена in инспекторе Craft_Window!");
            }
        }

        private void ClearItemButtons()
        {
            foreach (var btn in _spawnedItemButtons) Destroy(btn);
            _spawnedItemButtons.Clear();
        }

        private void ClearMaterialSlots()
        {
            foreach (var slot in _spawnedMaterialSlots) Destroy(slot);
            _spawnedMaterialSlots.Clear();
        }

        private void ClearInspector()
        {
            _currentlySelectedItem = null;
            if (selectedItemIcon != null) { selectedItemIcon.sprite = null; selectedItemIcon.enabled = false; }
            if (selectedItemDescription != null) selectedItemDescription.text = "Выберите предмет из списка...";
            if (btnCreate != null) btnCreate.interactable = false;
            ClearMaterialSlots();
        }

        public void ResetToFirstCascadeStep()
        {
            ClearItemButtons();
            ClearMaterialSlots();
            ClearInspector();

            _currentlySelectedItem = null;

            if (panel1Categories != null) panel1Categories.SetActive(true);
            if (panel2ItemsList != null) panel2ItemsList.SetActive(false);
            if (panel3RecipeInspector != null) panel3RecipeInspector.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
}