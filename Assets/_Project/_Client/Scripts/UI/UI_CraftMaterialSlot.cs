using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProjectCraft._Shared;
using ProjectCraft._Server;

namespace ProjectCraft._Client.UI
{
    public class UI_CraftMaterialSlot : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image materialIcon;
        [SerializeField] private TextMeshProUGUI countText;

        [Header("Design Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color missingColor = Color.red; // Красный, если материалов не хватает


        public void SetupMaterialSlot(ResourceCost costData, ServerPlayerEntity player)
        {
            if (costData.resource == null || player == null || player.Inventory == null)
            {
                Destroy(gameObject);
                return;
            }

            if (materialIcon != null)
            {
                materialIcon.sprite = costData.resource.itemIcon;
            }


            int currentInInventory = player.Inventory.GetItemCount(costData.resource);
            int requiredCount = costData.count;

            if (countText != null)
            {
                countText.text = $"{requiredCount} / {currentInInventory}";

                if (currentInInventory < requiredCount)
                {
                    countText.color = missingColor;
                }
                else
                {
                    countText.color = normalColor;
                }
            }
        }
    }
}


       
