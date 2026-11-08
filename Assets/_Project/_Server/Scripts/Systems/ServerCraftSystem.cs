using UnityEngine;
using ProjectCraft._Shared;

namespace ProjectCraft._Server
{
    public class ServerCraftSystem : MonoBehaviour
    {

        public void CraftItem(ServerPlayerEntity player, UniversalItemData blueprint)
        {
            if (player == null || player.Inventory == null || blueprint == null) return;


            if (blueprint.requiredMaterials != null)
            {
                foreach (var materialCost in blueprint.requiredMaterials)
                {
                    if (player.Inventory.GetItemCount(materialCost.resource) < materialCost.count)
                    {
                        Debug.LogWarning($"Недостаточно материалов для крафта: {materialCost.resource.itemName}!");
                        return;
                    }
                }
            }

            if (!player.Inventory.HasEnoughFreeSlots(false, 1))
            {
                Debug.LogWarning("Недостаточно свободного места в общем инвентаре!");
                return;
            }

            if (blueprint.requiredMaterials != null)
            {
                foreach (var materialCost in blueprint.requiredMaterials)
                {
                    player.Inventory.RemoveItem(materialCost.resource, materialCost.count);
                }
            }

            Rarity rolledRarity = RollRarityTier();

            bool success = player.Inventory.AddItem(blueprint, 1, 0);

            if (success)
            {
                Debug.Log($"[ServerCraft] Игрок {player.name} успешно скрафтил предмет: {blueprint.itemName} [{rolledRarity}]");

                player.Inventory.InvokeOnInventoryChanged(false);
            }
        }

        private Rarity RollRarityTier()
        {
            int rng = UnityEngine.Random.Range(1, 101);

            if (rng <= 2) return Rarity.Legendary;
            if (rng <= 10) return Rarity.Epic;   
            if (rng <= 30) return Rarity.Rare;

            return Rarity.Common;
        }
    }
}
