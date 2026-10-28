using System;
using UnityEngine;

namespace ProjectCraft._Shared
{
    [Serializable]
    public struct InventorySlot
    {
        public UniversalItemData itemData; // Ссылка на ScriptableObject предмета
        public int count;                 

        public InventorySlot(UniversalItemData item, int count)
        {
            this.itemData = item;
            this.count = count;
        }

        public bool IsEmpty => itemData == null || count <= 0;

        public void Clear()
        {
            itemData = null;
            count = 0;
        }
    }
}
