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
        private int _customParam;
        

        private SlotType _expectedSlotType;
        private IItemSlotClickHandler _windowController;
        private bool _hasItem;

        public void SetupSlot(int id, int customParam, IItemSlotClickHandler controller)
        {
            _slotId = id;
            _customParam = customParam;
            _windowController = controller;
        }

        public void Render(InventorySlot slotData)
        {
            // 1. Проверяем базовую пустоту слота
            if (slotData.IsEmpty)
            {
                ClearSlot();
                return;
            }

            // 2. Адаптивно определяем спрайт (для шмота или для скиллов)
            Sprite targetSprite = null;

            if (slotData.skillData != null)
            {
                targetSprite = slotData.skillData.skillIcon;
            }
            else if (slotData.itemData != null)
            {
                targetSprite = slotData.itemData.itemIcon;
            }

            if (targetSprite == null)
            {
                ClearSlot();
                return;
            }

     
            _hasItem = true;
            if (iconImage != null)
            {
                iconImage.enabled = true;
                iconImage.sprite = targetSprite;
            }

            if (emptyPlaceholder != null)
            {
                emptyPlaceholder.SetActive(false);
            }
        }

        private void ClearSlot()
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
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_hasItem || _windowController == null) return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _windowController.OnSlotRightClicked(_slotId, _customParam);
            }
        }
        public int SlotId => _slotId;
    }
}