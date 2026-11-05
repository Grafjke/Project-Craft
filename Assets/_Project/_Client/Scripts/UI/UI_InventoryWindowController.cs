using System.Collections.Generic;
using UnityEngine;
using ProjectCraft._Server;
using ProjectCraft._Shared;
using UnityEngine.UI;

namespace ProjectCraft._Client.UI
{

    public class UI_InventoryWindowController : MonoBehaviour
    {

        [Header("References")]
        [SerializeField] private ServerPlayerEntity targetPlayer;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform itemsGridParent;
        [SerializeField] private Button btnItemsTab;
        [SerializeField] private Button btnSkillsTab;

        private readonly List<UI_InventorySlot> _uiSlots = new List<UI_InventorySlot>();
        private bool _isViewingSkillsTab = false;
        private ServerPlayerInventory _subscribedInventory;

        private void Start()
        {
            if (btnItemsTab != null)
            {
                btnItemsTab.onClick.RemoveAllListeners();
                btnItemsTab.onClick.AddListener(() => ToggleTab(false));
            }

            if (btnSkillsTab != null)
            {
                btnSkillsTab.onClick.RemoveAllListeners();
                btnSkillsTab.onClick.AddListener(() => ToggleTab(true));
            }

            EnsurePlayerConnection();
            ToggleTab(false); 
        }

        private void OnEnable()
        {
            EnsurePlayerConnection();
            RefreshUI();
        }

        private void Update()
        {

            if (_subscribedInventory == null)
            {
                EnsurePlayerConnection();
            }
        }

        private void OnDestroy()
        {
            if (_subscribedInventory != null)
            {
                _subscribedInventory.OnInventoryChanged -= HandleInventoryChanged;
            }
        }

        private void EnsurePlayerConnection()
        {
            if (targetPlayer == null)
            {
                targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();
            }

            if (targetPlayer != null && targetPlayer.Inventory != null)
            {
                TrySubscribeToInventory();
            }
        }

        private void TrySubscribeToInventory()
        {
            if (targetPlayer == null || targetPlayer.Inventory == null) return;
            if (_subscribedInventory == targetPlayer.Inventory) return;

            if (_subscribedInventory != null)
            {
                _subscribedInventory.OnInventoryChanged -= HandleInventoryChanged;
            }

            _subscribedInventory = targetPlayer.Inventory;
            _subscribedInventory.OnInventoryChanged += HandleInventoryChanged;

            RefreshUI();
        }

        private void HandleInventoryChanged(bool isSkillTab)
        {
            if (isSkillTab == _isViewingSkillsTab)
            {
                RefreshUI();
            }
        }

        public void ToggleTab(bool viewSkills)
        {
            _isViewingSkillsTab = viewSkills;
            int requiredSize = _isViewingSkillsTab ? 50 : 100;

            while (_uiSlots.Count < requiredSize)
            {
                GameObject newSlotGo = Instantiate(slotPrefab, itemsGridParent);
                UI_InventorySlot slotScript = newSlotGo.GetComponent<UI_InventorySlot>();
                if (slotScript != null) _uiSlots.Add(slotScript);
            }

            for (int i = 0; i < _uiSlots.Count; i++)
            {
                if (i < requiredSize)
                {
                    _uiSlots[i].gameObject.SetActive(true);
                    _uiSlots[i].Setup(i, _isViewingSkillsTab, this);
                }
                else
                {
                    _uiSlots[i].gameObject.SetActive(false);
                }
            }

            if (btnItemsTab != null) btnItemsTab.interactable = _isViewingSkillsTab;
            if (btnSkillsTab != null) btnSkillsTab.interactable = !_isViewingSkillsTab;

            EnsurePlayerConnection();
            RefreshUI();
            if (itemsGridParent != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(itemsGridParent.GetComponent<RectTransform>());

                if (itemsGridParent.parent != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(itemsGridParent.parent.GetComponent<RectTransform>());
                }
            }
        }

        public void RefreshUI()
        {
            if (targetPlayer == null || targetPlayer.Inventory == null) return;

            for (int i = 0; i < _uiSlots.Count; i++)
            {
                InventorySlot serverSlotData = targetPlayer.Inventory.GetSlot(i, _isViewingSkillsTab);
                _uiSlots[i].Render(serverSlotData);
            }
        }
        public void RequestEquipItem(int slotIndex)
        {
            if (targetPlayer == null || targetPlayer.Inventory == null || targetPlayer.Equipment == null) return;

            InventorySlot slotData = targetPlayer.Inventory.GetSlot(slotIndex, _isViewingSkillsTab);

            if (slotData.IsEmpty) return;

            if (_isViewingSkillsTab)
            {
                if (slotData.itemData != null && slotData.itemData.itemType == ItemType.Capsule)
                {
                    bool success = targetPlayer.Inventory.Server_OpenSummonCapsule(slotIndex, 1);

                    if (success)
                    {
                        Debug.Log($"Успешно отправлен запрос на открытие 1 капсулы из слота №{slotIndex}");
                    }
                    return;
                }
                return;
            }

            if (slotData.itemData != null && slotData.itemData.itemType == ItemType.Equipment)
            {
                int targetSlotId = (int)slotData.itemData.slotID;
                bool success = targetPlayer.Equipment.EquipItem(slotIndex, targetSlotId, targetPlayer.Inventory, slotData.itemData);

                if (success)
                {
                    Debug.Log($"Предмет {slotData.itemData.itemName} экипирован в слот {slotData.itemData.slotID}");
                }
            }
            else
            {
                targetPlayer.SendSystemMessage("Этот предмет нельзя экипировать!");
            }
        }
    }
}
