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
                    if (!targetTab[i].IsEmpty && targetTab[i].itemData != null && targetTab[i].itemData.itemID == item.itemID && targetTab[i].count < maxStack)
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

        public bool AddSkill(UniversalSkillData skill)
        {
            if (skill == null) return false;

            for (int i = 0; i < _skillTab.Length; i++)
            {
                if (_skillTab[i].IsEmpty)
                {
                    _skillTab[i] = new InventorySlot(skill);
                    OnInventoryChanged?.Invoke(true);
                    return true;
                }
            }
            return false;
        }
        public bool Server_OpenSummonCapsule(int indexInSkillTab, int amountToOpen)
        {
            if (indexInSkillTab < 0 || indexInSkillTab >= _skillTab.Length) return false;

            InventorySlot slotData = _skillTab[indexInSkillTab];

            if (slotData.IsEmpty || slotData.itemData == null || slotData.itemData.itemType != ItemType.Capsule) return false;

            if (amountToOpen <= 0 || amountToOpen > slotData.count) return false;

            if (slotData.itemData.containedSkills == null || slotData.itemData.containedSkills.Length == 0) return false;

            int slotsFreedByCapsule = (slotData.count == amountToOpen) ? 1 : 0;
            int netRequiredSlots = amountToOpen - slotsFreedByCapsule;

            if (netRequiredSlots > 0 && !HasEnoughFreeSlots(true, netRequiredSlots))
            {
                return false;
            }

            UniversalSkillData[] pool = slotData.itemData.containedSkills;

            slotData.count -= amountToOpen;
            if (slotData.count <= 0)
            {
                _skillTab[indexInSkillTab] = default;
            }
            else
            {
                _skillTab[indexInSkillTab] = slotData; 
            }
            
            for (int i = 0; i < amountToOpen; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, pool.Length);
                UniversalSkillData rolledSkill = pool[randomIndex];

                AddSkill(rolledSkill);
            }

            OnInventoryChanged?.Invoke(true);
            return true;
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

        public void InvokeOnInventoryChanged(bool isSkillTab)
        {
            OnInventoryChanged?.Invoke(isSkillTab);
        }

        public int GetItemCount(UniversalItemData targetItem)
        {
            if (targetItem == null) return 0;

            int totalCount = 0;


            for (int i = 0; i < _itemTab.Length; i++)
            {
                if (_itemTab[i].IsEmpty) continue;

                if (_itemTab[i].itemData != null && _itemTab[i].itemData.itemID == targetItem.itemID)
                {
                    totalCount += _itemTab[i].count;
                }
            }

            return totalCount;
        }



    }
}