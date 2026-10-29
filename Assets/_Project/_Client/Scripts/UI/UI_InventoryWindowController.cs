using System.Collections.Generic;
using UnityEngine;
using ProjectCraft._Server;
using ProjectCraft._Shared;

namespace ProjectCraft._Client.UI
{

    public class UI_InventoryWindowController : MonoBehaviour
    {

        [Header("References")]
        [SerializeField] private ServerPlayerEntity targetPlayer; 
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform itemsGridParent;

       
        private readonly List<UI_InventorySlot> _uiSlots = new List<UI_InventorySlot>();

        private bool _isViewingSkillsTab = false;

        private void Start()
        {
            
            if (targetPlayer == null)
            {
                targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();
            }
            ToggleTab(false);
        }

        private void OnEnable()
        {
            RefreshUI();
        }

        public void ToggleTab(bool viewSkills)
        {
            _isViewingSkillsTab = viewSkills;

          
            int requiredSize = _isViewingSkillsTab ? 50 : 100;

           
            foreach (Transform child in itemsGridParent) Destroy(child.gameObject);
            _uiSlots.Clear();

            for (int i = 0; i < requiredSize; i++)
            {
                GameObject newSlotGo = Instantiate(slotPrefab, itemsGridParent);
                UI_InventorySlot slotScript = newSlotGo.GetComponent<UI_InventorySlot>();

                if (slotScript != null)
                {
                    slotScript.Setup(i, _isViewingSkillsTab);
                    _uiSlots.Add(slotScript);
                }
            }

            RefreshUI();
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
    }
}
