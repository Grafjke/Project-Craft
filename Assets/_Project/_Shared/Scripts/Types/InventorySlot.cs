using System;
using UnityEngine;

namespace ProjectCraft._Shared
{
    [Serializable]
    public struct InventorySlot
    {
        public string itemInstanceId;
        public UniversalItemData itemData;
        public UniversalSkillData skillData;
        public int count;
        public bool isLocked;


        public InventorySlot(UniversalItemData item, int count)
        {
            this.itemData = item;
            this.skillData = null;
            this.count = count;
            this.isLocked = false;
            this.itemInstanceId = null;
        }
        public InventorySlot(UniversalSkillData skill)
        {
            this.itemData = null;
            this.skillData = skill;
            this.count = 1;
            this.isLocked = false;
            this.itemInstanceId = Guid.NewGuid().ToString();
        }



        public bool IsEmpty => (itemData == null && skillData == null) || count <= 0;

        public void Clear()
        {
            itemData = null;
            skillData = null;
            count = 0;
            itemInstanceId = null;
            isLocked = false;
        }
    }
}
