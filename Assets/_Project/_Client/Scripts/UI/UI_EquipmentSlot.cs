using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ProjectCraft._Shared;

namespace ProjectCraft._Client.UI
{
    public class UI_EquipmentSlot : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject emptyPlaceholder;

        private int _slotId;
        private SlotType _expectedSlotType;
        private UI_CharacterWindowController _windowController;
        private bool _hasItem;

        public void SetupSlot(int id, SlotType slotType, UI_CharacterWindowController controller)
        {
            _slotId = id;
            _expectedSlotType = slotType;
            _windowController = controller;
        }

        public void Render(InventorySlot slotData)
        {
            if (slotData.IsEmpty || slotData.itemData == null)
            {
                _hasItem = false; 
                if (iconImage != null)
                {
                    iconImage.enabled = false;
                    iconImage.sprite = null;
                }

                if (emptyPlaceholder != null)
                {
                    emptyPlaceholder.SetActive(true);
                }
                return;
            }

            _hasItem = true;
            if (iconImage != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = slotData.itemData.itemIcon;
            }

            if (emptyPlaceholder != null)
            {
                emptyPlaceholder.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_hasItem || _windowController == null) return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _windowController.RequestUnequip(_slotId);
            }
        }
        public int SlotId => _slotId;
    }
}