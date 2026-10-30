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

        public const int SLOT_HEAD = 0;
        public const int SLOT_BODY = 1;
        public const int SLOT_GLOVES = 2;
        public const int SLOT_BOOTS = 3;
        public const int SLOT_NECK = 4;
        public const int SLOT_RING_1 = 5;
        public const int SLOT_RING_2 = 6;
        public const int SLOT_MAIN_HAND = 7;
        public const int SLOT_OFF_HAND = 8;

        public event Action<int> OnEquipmentChanged;

        public ServerPlayerEquipment(ServerPlayerEntity player)
        {
            _player = player;
            for (int i = 0; i <= 8; i++)
            {
                _slots[i] = default;
            }
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

            if (itemData.slotID != (SlotType)targetSlotId)
            {

                if ((itemData.slotID == SlotType.Ring1 || itemData.slotID == SlotType.Ring2) &&
                    (targetSlotId == SLOT_RING_1 || targetSlotId == SLOT_RING_2))
                {
                    // Пропускаем, валидация колец пройдена
                }
                else
                {
                    _player.SendSystemMessage("Этот предмет нельзя надеть в данный слот!");
                    return false;
                }
            }


            if (targetSlotId == SLOT_MAIN_HAND && itemData.handednessType == HandednessType.Two_Handed)
            {
                InventorySlot offHandSlot = _slots[SLOT_OFF_HAND];
                if (!offHandSlot.IsEmpty)
                {
                    if (!inventory.AddItem(offHandSlot.itemData, offHandSlot.count))
                    {
                        _player.SendSystemMessage("Освободите инвентарь, чтобы снять предмет из левой руки!");
                        return false;
                    }
                    _slots[SLOT_OFF_HAND] = default;
                    OnEquipmentChanged?.Invoke(SLOT_OFF_HAND);
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

            invSlot.count = 1;
            _slots[targetSlotId] = invSlot;

            if (!equippedBefore.IsEmpty)
            {
                inventory.SetSlot(fromInvIndex, equippedBefore, false);
            }
            else
            {
                inventory.ClearSlot(fromInvIndex, false);
            }

            _player.RecalculateModifiersCache();
            OnEquipmentChanged?.Invoke(targetSlotId);
            return true;
        }

        public bool UnequipItem(int slotId, ServerPlayerInventory inventory)
        {
            if (_player.IsInCombat)
            {
                _player.SendSystemMessage("Нельзя менять экипировку в бою!");
                return false;
            }

            InventorySlot equippedItem = _slots[slotId];
            if (equippedItem.IsEmpty) return false;

            if (!inventory.AddItem(equippedItem.itemData, equippedItem.count))
            {
                _player.SendSystemMessage("Инвентарь заполнен!");
                return false;
            }

            _slots[slotId] = default;

            if (slotId == SLOT_MAIN_HAND && equippedItem.itemData.handednessType == HandednessType.Two_Handed)
            {
                IsOffHandBlocked = false;
            }

            _player.RecalculateModifiersCache();
            OnEquipmentChanged?.Invoke(slotId);
            return true;
        }

        public Dictionary<int, InventorySlot> GetPaperdoll() => _slots;
    }
}