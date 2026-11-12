using ProjectCraft._Shared;
using UnityEngine;

namespace ProjectCraft._Server
{
    public interface IDamageable
    {
        Transform Transform { get; }
        Transform HitTargetPoint { get; }

        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }
        bool IsImmune { get; }

        float GetStatValue(StatType statType);
        void TakeDamage(DamageInfo damageInfo);
    }

    public struct DamageInfo
    {
        public IDamageable Attacker;
        public float BaseDamage;
        public DamageType DamageType;
        public ScalingAttribute ScalingAttribute;
        public string[] ElementalTags;
        public bool CanCrit;
    }
}