using ProjectCraft._Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        public void Setup(int index, bool isSkill)
        {
            SlotIndex = index;
            IsSkillSlot = isSkill;
        }

        public void Render(InventorySlot data)
        {
            if (data.IsEmpty || data.itemData == null)
            {
                iconImage.gameObject.SetActive(false);
                countText.gameObject.SetActive(false);
                if (lockOverlay != null) lockOverlay.gameObject.SetActive(false);
                return;
            }

            
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
    }
}