using ProjectCraft._Shared;
using System;

namespace ProjectCraft._Server
{
    public class ServerPlayerInventory
    {
        private const int ITEMS_TAB_SIZE = 100;
        private const int SKILL_TAB_SIZE = 50;

        private readonly InventorySlot[] _itemTab = new InventorySlot[ITEMS_TAB_SIZE];
        private readonly InventorySlot[] _skillTab = new InventorySlot[SKILL_TAB_SIZE];

        public event Action<bool> OnInventoryChanged;

        public ServerPlayerInventory()
        {
            ClearAll();
            OnInventoryChanged?.Invoke(true);
            OnInventoryChanged?.Invoke(false);
        }

        public void ClearAll()
        {
            for (int i = 0; i < _itemTab.Length; i++) _itemTab[i] = default;
            for (int i = 0; i < _skillTab.Length; i++) _skillTab[i] = default;
        }

        
        public bool HasEnoughFreeSlots(bool isSkillTab, int requiredSlots)
        {
            InventorySlot[] targetTab = isSkillTab ? _skillTab : _itemTab;
            int freeCount = 0;

            foreach (var slot in targetTab)
            {
                if (slot.IsEmpty) freeCount++;
            }

            return freeCount >= requiredSlots;
        }


        public bool AddItem(UniversalItemData item, int count, int upgradeLevel = 0)
        {
            if (item == null || count <= 0) return false;

            bool isSkill = (item.itemType == ItemType.Capsule);
            var targetTab = isSkill ? _skillTab : _itemTab;
            int maxStack = item.maxStackValue;

            if (maxStack > 1)
            {
                for (int i = 0; i < targetTab.Length; i++)
                {
                    if (!targetTab[i].IsEmpty && targetTab[i].itemData.itemID == item.itemID && targetTab[i].count < maxStack)
                    {
                        int space = maxStack - targetTab[i].count;
                        int addAmount = Math.Min(space, count);

                        targetTab[i].count += addAmount;
                        count -= addAmount;

                        if (count <= 0)
                        {
                            OnInventoryChanged?.Invoke(isSkill);
                            return true;
                        }
                    }
                }
            }

            for (int i = 0; i < targetTab.Length; i++)
            {
                if (targetTab[i].IsEmpty)
                {
                    int addAmount = Math.Min(maxStack, count);

                    
                    targetTab[i] = new InventorySlot
                    {
                        itemInstanceId = System.Guid.NewGuid().ToString(),
                        itemData = item,
                        count = addAmount,
                        isLocked = false
                    };

                    count -= addAmount;
                    if (count <= 0)
                    {
                        OnInventoryChanged?.Invoke(isSkill);
                        return true;
                    }
                }
            }

            return false;
        }

        public InventorySlot GetSlot(int index, bool isSkillTab)
        {

            var targetTab = isSkillTab ? _skillTab : _itemTab;
            if (index < 0 || index >= targetTab.Length) return default;
            return targetTab[index];
        }

        public void SetSlot(int index, InventorySlot slot, bool isSkillTab)
        {
            var targetTab = isSkillTab ? _skillTab : _itemTab;
            if (index >= 0 && index < targetTab.Length) targetTab[index] = slot;
        }

        public void ClearSlot(int index, bool isSkillTab)
        {
            SetSlot(index, default, isSkillTab);
        }
    }
}