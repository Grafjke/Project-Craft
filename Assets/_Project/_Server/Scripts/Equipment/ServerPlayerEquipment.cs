using System;
using System.Collections.Generic;
using ProjectCraft._Shared;


namespace ProjectCraft._Server
{
    public class ServerPlayerEquipment
    {
        private readonly ServerPlayerEntity _player;

        private readonly Dictionary<int, InventorySlot> _slots = new Dictionary<int, InventorySlot>();
    
        public bool IsOffHandBlocked { get; private set; }
        public const int SLOT_HEAD = 1;
        public const int SLOT_BODY = 2;
        public const int SLOT_GLOVES = 3;
        public const int SLOT_LEGS = 4;
        public const int SLOT_NECK = 5;
        public const int SLOT_RING_1 = 6;
        public const int SLOT_RING_2 = 7;
        public const int SLOT_MAIN_HAND = 8;
        public const int SLOT_OFF_HAND = 9;
        public ServerPlayerEquipment(ServerPlayerEntity player)
        {
            _player = player;
            for (int i = 1; i <= 9; i++) _slots[i] = default;
        }

        public bool EquipItem(int fromInvIndex, int targetSlotId, ServerPlayerInventory inventory, UniversalItemData itemData)
        {

            if (_player.IsInCombat)
            {
                _player.SendSystemMessage("Нельзя менять экипировку в бою!");
                return false;
            }

            InventorySlot invSlot = inventory.GetSlot(fromInvIndex, false);
            if (invSlot.IsEmpty || invSlot.isLocked) return false;

            
            if (_player.CharacterLevel < itemData.requiredLevel)
            {
                _player.SendSystemMessage("Недостаточный уровень!");
                return false;
            }

            
            if (targetSlotId == SLOT_MAIN_HAND && itemData.handednessType == HandednessType.Two_Handed)
            {
                InventorySlot offHandItem = _slots[SLOT_OFF_HAND];
                if (!offHandItem.IsEmpty)
                {
                   
                    if (!inventory.AddItem(GetItemDataMock(offHandItem.itemData.itemID), offHandItem.itemData.maxStackValue, offHandItem.itemData.upgradeLvl))
                    {
                        _player.SendSystemMessage("Освободите инвентарь, чтобы снять предмет из левой руки!");
                        return false;
                    }
                    _slots[SLOT_OFF_HAND] = default;
                }
                IsOffHandBlocked = true;
            }
            else if (targetSlotId == SLOT_OFF_HAND && IsOffHandBlocked)
            {
                _player.SendSystemMessage("Левая рука заблокирована двуручным оружием!");
                return false;
            }

            if (targetSlotId == SLOT_MAIN_HAND && itemData.handednessType == HandednessType.One_Handed)
            {
                IsOffHandBlocked = false;
            }

            
            InventorySlot equippedBefore = _slots[targetSlotId];

            _slots[targetSlotId] = new InventorySlot
            {
                itemData = itemData,
                count = 1
            };

            if (!equippedBefore.IsEmpty)
            {
                inventory.SetSlot(fromInvIndex, equippedBefore, false);
            }
            else
            {
                inventory.ClearSlot(fromInvIndex, false);
            }

            
            _player.RecalculateModifiersCache();
            return true;
        }

        public Dictionary<int, InventorySlot> GetPaperdoll() => _slots;

        private UniversalItemData GetItemDataMock(string id) => null; // Здесь будет подгрузка ScriptableObject по ID
    }
}










