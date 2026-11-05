using ProjectCraft._Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProjectCraft._Client.UI
{
    public class UI_InventorySlot : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image iconImage; 
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Image lockOverlay; // Пока не используем, но задел на будущее

        public int SlotIndex { get; private set; }
        public bool IsSkillSlot { get; private set; }
        private UI_InventoryWindowController _windowController;
        private bool _hasItem;

        public void Setup(int index, bool isSkill, UI_InventoryWindowController controller)
        {
            SlotIndex = index;
            IsSkillSlot = isSkill;
            _windowController = controller;
        }

        public void Render(InventorySlot data)
        {
            if (data.IsEmpty || data.itemData == null)
            {
                _hasItem = false;
                iconImage.gameObject.SetActive(false);
                countText.gameObject.SetActive(false);
                if (lockOverlay != null) lockOverlay.gameObject.SetActive(false);
                return;
            }

            _hasItem = true;
            
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = data.itemData.itemIcon;

            
            if (data.count > 1)
            {
                countText.gameObject.SetActive(true);
                countText.text = data.count.ToString();
            }
            else
            {
                countText.gameObject.SetActive(false);
            }

            if (lockOverlay != null) lockOverlay.gameObject.SetActive(data.isLocked);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"[Slot Debug] Кликнут слот №{SlotIndex}. Кнопка мыши: {eventData.button}");
            if (!_hasItem || _windowController == null) return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _windowController.RequestEquipItem(SlotIndex);
            }
        }
    }
}