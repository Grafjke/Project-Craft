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

        
        private Dictionary<string, List<UniversalItemData>> _recipesByCategory = new Dictionary<string, List<UniversalItemData>>();
        private List<GameObject> _spawnedItemButtons = new List<GameObject>();
        private List<GameObject> _spawnedMaterialSlots = new List<GameObject>();

        private UniversalItemData _currentlySelectedItem;

        private void Awake()
        {
            LoadRecipesFromProject();

            if (btnCreate != null) btnCreate.onClick.AddListener(RequestCraft);
        }

        private void OnEnable()
        {
            if (targetPlayer == null) targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();
            ClearInspector();
        }

        private void LoadRecipesFromProject()
        {
            _recipesByCategory.Clear();

            UniversalItemData[] allEquipment = Resources.LoadAll<UniversalItemData>("ScriptableObjects/Items/Equipments");

            foreach (var item in allEquipment)
            {
                if (item == null) continue;

                // Сортируем по папкам/категориям (используем Weapon_Or_Armor_Tag, например: "Sword", "Staff", "Plate")
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

            foreach (var recipe in _recipesByCategory[categoryName])
            {
                GameObject btnGo = Instantiate(itemButtonPrefab, itemsGridParent);

                var textComponent = btnGo.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null) textComponent.text = recipe.itemName;

                var btn = btnGo.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.AddListener(() => SelectItemForCrafting(recipe));
                }

                _spawnedItemButtons.Add(btnGo);
            }
        }

        public void SelectItemForCrafting(UniversalItemData item)
        {
            ClearMaterialSlots();
            _currentlySelectedItem = item;

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

                foreach (var stat in item.staticStatsBlueprint)
                {
                    sb.AppendLine($"• {stat.statName}: +{stat.baseValue}");
                }
                selectedItemDescription.text = sb.ToString();
            }

            foreach (var reqMaterial in item.requiredMaterials)
            {
                GameObject matSlotGo = Instantiate(materialSlotPrefab, materialsGridParent);

                // Здесь будет логика UI_CraftMaterialSlot: подгрузка иконки ресурса по его ID,
                // подсчет количества у игрока (например, "5 / 12") и покраска текста в красный, если не хватает.

                _spawnedMaterialSlots.Add(matSlotGo);
            }

            if (btnCreate != null) btnCreate.interactable = true;
        }

        private void RequestCraft()
        {
            if (_currentlySelectedItem == null || targetPlayer == null) return;
            Debug.Log($"Отправлен запрос на создание {_currentlySelectedItem.itemName}");
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
    }
}