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

        [Header("Weapon Combat Settings (If Main_Hand)")]
        public AttackType attackType = AttackType.Melee;
        public DamageType damageType = DamageType.Physical;
        public ScalingAttribute scalingAttribute = ScalingAttribute.Strength;
        public float baseWeaponDamage = 10f;
        public float attackRange = 2.0f;
        public float baseAttackCooldown = 1.5f;
        public GameObject projectilePrefab;

        
        [Header("Stacking Settings")]
        [Range(1, 99)]
        public int maxStackValue = 1;

        [Header("Stats Blueprint (For Equipment)")]
        public StatModifier[] staticStatsBlueprint;

        [Header("Crafting Recipe (For Forge NPC)")]
        public ResourceCost[] requiredMaterials;

        [Header("Capsule Content Settings")]
        [Tooltip("Пул навыков (a, b, c)")]
        public UniversalSkillData[] containedSkills;
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