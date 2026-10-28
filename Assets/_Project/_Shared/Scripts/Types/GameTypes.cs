using UnityEngine;

namespace ProjectCraft._Shared
{
   
    public enum ItemType
    {
        Equipment,
        Resource,
        Capsule
    }

   
    public enum SlotID
    {
        Head,
        Body,
        Gloves,
        Boots,
        Neck,
        Ring1,
        Ring2,
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
}
