using ProjectCraft._Shared;
using System;
using UnityEngine;

namespace ProjectCraft._Server
{
    public class ServerEnemyEntity : MonoBehaviour, IDamageable
    {
        [Header("Data & Anchors")]
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private Transform hitTargetPoint;
        [SerializeField] private Transform defaultProjectileSpawnPoint;

        public EnemyData Data => enemyData;
        public Transform Transform => transform;
        public Transform HitTargetPoint => hitTargetPoint != null ? hitTargetPoint : transform;
        public Transform ProjectileSpawnPoint => defaultProjectileSpawnPoint != null ? defaultProjectileSpawnPoint : transform;
        public string EntityName => enemyData != null && !string.IsNullOrEmpty(enemyData.enemyName)
        ? enemyData.enemyName
        : gameObject.name;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => enemyData != null ? enemyData.maxHealth : 100f;
        public bool IsDead { get; private set; }
        public bool IsImmune { get; set; }

        public event Action<float, float> OnHealthChanged;
        public event Action<IDamageable> OnDamageTaken;
        public event Action OnEnemyDied;

        private void Awake()
        {
            if (enemyData != null)
            {
                CurrentHealth = enemyData.maxHealth;
            }
        }

        public void ResetEntity()
        {
            IsDead = false;
            IsImmune = false;
            CurrentHealth = MaxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (IsDead || IsImmune) return;

            float finalDamage = DamageCalculator.CalculateFinalDamage(damageInfo, this, out bool isCrit);
            CurrentHealth = Mathf.Clamp(CurrentHealth - finalDamage, 0f, MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            Debug.Log($"[Combat] {enemyData.enemyName} получил {finalDamage} урона {(isCrit ? "(КРИТ!)" : "")}. HP: {CurrentHealth}/{MaxHealth}");

            if (CurrentHealth <= 0f)
            {
                Die();
            }
            else
            {
                OnDamageTaken?.Invoke(damageInfo.Attacker);
            }
        }

        public float GetStatValue(StatType statType)
        {
            if (enemyData == null) return 0f;

            return statType switch
            {
                StatType.Armor => enemyData.armor,
                StatType.Fire_Resistance => enemyData.elementalResistance,
                StatType.Lightning_Resistance => enemyData.elementalResistance,
                StatType.Shadow_Resistance => enemyData.elementalResistance,
                _ => 0f
            };
        }

        private void Die()
        {
            IsDead = true;
            Debug.Log($"[Combat] {enemyData.enemyName} повержен!");
            OnEnemyDied?.Invoke();

            gameObject.tag = "Untagged";
            if (TryGetComponent<Collider>(out var col)) col.enabled = false;
            Destroy(gameObject, 2f);
        }
    }
}