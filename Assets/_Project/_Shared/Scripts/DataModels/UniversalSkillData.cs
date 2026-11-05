using UnityEngine;
using ProjectCraft._Shared;

namespace ProjectCraft._Shared
{
    [CreateAssetMenu(fileName = "NewSkillData", menuName = "GDD/Universal Skill Data")]
    public class UniversalSkillData : ScriptableObject
    {
        [Header("Base Data")]
        public string skillID;
        public string skillName;
        public Sprite skillIcon;
        public bool isPassive;
       

        [Header("Combat Settings")]
        public float castTime;
        public float cooldown;
        public int manaCost;

        [Header("Tags & Synergy")]
        public string[] requiredWeaponTags;
        public string[] elementalTags;

        [Header("Effects")]
        public float movementDistance;
        public string statusEffectToApply;  // ID накладываемого эффекта (напр. Stun_1.5sec)

        [Header("Damage Formula")]
        public string baseDamageFormulaType; 
        public float baseDamageValue;        
    }
}