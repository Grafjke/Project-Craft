using ProjectCraft._Shared;
using UnityEngine;

namespace ProjectCraft._Server
{
    public static class DamageCalculator
    {
        public static float CalculateFinalDamage(DamageInfo info, IDamageable target, out bool isCrit)
        {
            isCrit = false;
            if (target == null || target.IsDead || target.IsImmune) return 0f;


            float rawDamage = info.BaseDamage;

            if (info.Attacker != null)
            {

                float attrMultiplier = GetAttributeMultiplier(info.Attacker, info.ScalingAttribute);

                float tagBonusPercent = GetTagBonusPercent(info.Attacker, info.DamageType);

                rawDamage = (info.BaseDamage * attrMultiplier) * (1f + tagBonusPercent / 100f);

                if (info.CanCrit)
                {
                    float critChance = info.Attacker.GetStatValue(StatType.Crit_Chance);
                    if (Random.Range(0f, 100f) <= critChance)
                    {
                        isCrit = true;
                        float critDamagePercent = info.Attacker.GetStatValue(StatType.Crit_Damage);
                        rawDamage *= (critDamagePercent / 100f);
                    }
                }
            }

            float absorption = CalculateAbsorption(target, info.DamageType);

            float finalDamage = rawDamage * (1f - absorption);
            return Mathf.Max(1f, Mathf.Round(finalDamage));
        }

        private static float GetAttributeMultiplier(IDamageable attacker, ScalingAttribute attr)
        {
            float statVal = attr switch
            {
                ScalingAttribute.Strength => attacker.GetStatValue(StatType.Strength),
                ScalingAttribute.Dexterity => attacker.GetStatValue(StatType.Dexterity),
                ScalingAttribute.Intelligence => attacker.GetStatValue(StatType.Intelligence),
                _ => 10f
            };

            return Mathf.Max(0.1f, statVal / 10f);
        }

        private static float GetTagBonusPercent(IDamageable attacker, DamageType damageType)
        {
            return damageType switch
            {
                DamageType.Physical => attacker.GetStatValue(StatType.Physical_Damage_Percent_Bonus),
                DamageType.Fire => attacker.GetStatValue(StatType.Fire_Damage_Percent_Bonus),
                DamageType.Lightning => attacker.GetStatValue(StatType.Lightning_Damage_Percent_Bonus),
                DamageType.Shadow => attacker.GetStatValue(StatType.Shadow_Damage_Percent_Bonus),
                _ => 0f
            };
        }

        private static float CalculateAbsorption(IDamageable target, DamageType damageType)
        {
            if (damageType == DamageType.Physical)
            {
                float armor = target.GetStatValue(StatType.Armor);
                float physAbsorb = armor / (armor + 500f);
                return Mathf.Clamp(physAbsorb, 0f, 0.75f);
            }
            else
            {
                StatType resStat = damageType switch
                {
                    DamageType.Fire => StatType.Fire_Resistance,
                    DamageType.Lightning => StatType.Lightning_Resistance,
                    DamageType.Shadow => StatType.Shadow_Resistance,
                    _ => StatType.Fire_Resistance
                };

                float resist = target.GetStatValue(resStat);
                return Mathf.Clamp(resist / 100f, 0f, 0.75f);
            }
        }
    }
}