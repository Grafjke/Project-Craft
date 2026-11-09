using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectCraft._Shared;

namespace ProjectCraft._Client.UI
{
    public class UI_CraftItemButton : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private Image itemIconImage;
        [SerializeField] private Button actionButton;

        public void SetupButton(UniversalItemData recipe, System.Action onClickCallback)
        {
 
            if (itemNameText != null) itemNameText.text = recipe.itemName;

            if (itemIconImage != null && recipe.itemIcon != null)
            {
                itemIconImage.sprite = recipe.itemIcon;
                itemIconImage.enabled = true;
            }

            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(() => onClickCallback?.Invoke());
            }
        }
    }
}