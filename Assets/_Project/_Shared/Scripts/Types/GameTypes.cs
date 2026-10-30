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
        Ring1,
        Ring2,
        Main_Hand, 
        Off_Hand  
    }

  
    public enum HandednessType
    {
        One_Handed,
        Two_Handed,
        None
    }
    public enum StatType
    {
       
        Strength,   
        Dexterity,  
        Intelligence,           

       
        Crit_Chance,           
        Crit_Damage,           
        Attack_Cast_Speed,     
        Fire_Damage_Percent_Bonus,   
        Lightning_Damage_Percent_Bonus,
        Shadow_Damage_Percent_Bonus,   
        Physical_Damage_Percent_Bonus,

        
        Armor,                
        Fire_Resistance,       
        Lightning_Resistance,   
        Shadow_Resistance,      

    
        Max_Health,             
        HP_Regen,          
        Max_Mana,             
        MP_Regen             
    }
    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }
}
