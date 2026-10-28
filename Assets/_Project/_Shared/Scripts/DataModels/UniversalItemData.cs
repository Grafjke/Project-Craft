using ProjectCraft._Shared;
using UnityEngine;

namespace ProjectCraft._Shared
{
    [CreateAssetMenu(fileName = "NewUniversalItem", menuName = "GDD/Universal Item Data")]
    public class UniversalItemData : ScriptableObject
    {
        [Header("Base Info")]
        public string itemID;
        public string itemName;
        public Sprite itemIcon;
        public ItemType itemType;
        public Rarity itemRarity;
        public int upgradeLvl;

        [Header("Equipment Settings (If Equipment)")]
        public SlotType slotID;      
        public HandednessType handednessType; 
        public string weaponOrArmorTag;     
        public int requiredLevel = 1;

        [Header("Stacking Settings")]
        [Range(1, 99)]
        public int maxStackValue = 1;

        [Header("Stats Blueprint (For Equipment)")]
        public StatModifier[] staticStatsBlueprint;

        [Header("Crafting Recipe (For Forge NPC)")]
        public ResourceCost[] requiredMaterials;
    }

    [System.Serializable]
    public struct StatModifier
    {
        public StatType statName;
        public float baseValue;
    }

    [System.Serializable]
    public struct ResourceCost
    {
        public UniversalItemData resource;
        public int count;              
    }
    
}