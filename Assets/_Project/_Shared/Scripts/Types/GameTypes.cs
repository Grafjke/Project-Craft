using UnityEngine;

namespace ProjectCraft._Shared
{
   
    public enum ItemType
    {
        Equipment,
        Resource,
        Capsule
    }

   
    public enum SlotType
    {
        Head,
        Body,
        Gloves,
        Boots,
        Neck,
        Ring,
        Main_Hand, 
        Off_Hand,  
        None
    }

  
    public enum HandednessType
    {
        One_Handed,
        Two_Handed,
        None
    }
    public enum StatType
    {
        Max_Health,
        Armor,
        Strength,
        Intellect,
        Physical_Damage
    }
    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }
}
