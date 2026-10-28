using System;
using UnityEngine;

namespace ProjectCraft._Shared
{
    [Serializable]
    public struct InventorySlot
    {
        public string itemInstanceId;
        public UniversalItemData itemData;
        public int count;
        public bool isLocked;


        public InventorySlot(UniversalItemData item, int count)
        {
            this.itemData = item;
            this.count = count;
            isLocked = false;
            itemInstanceId = null;
        }

        public bool IsEmpty => itemData == null || count <= 0;

        public void Clear()
        {
            itemData = null;
            count = 0;
        }
    }
}
