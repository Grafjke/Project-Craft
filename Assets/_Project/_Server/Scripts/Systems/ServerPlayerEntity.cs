using ProjectCraft._Server;
using ProjectCraft._Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerEntity : MonoBehaviour, IDamageable
{
    [Header("Capsule Anchor Points")]
    [SerializeField] private Transform defaultProjectileSpawnPoint;
    [SerializeField] private Transform hitTargetPoint;

    public Transform Transform => transform;
    public Transform HitTargetPoint => hitTargetPoint != null ? hitTargetPoint : transform;
    public Transform ProjectileSpawnPoint => defaultProjectileSpawnPoint != null ? defaultProjectileSpawnPoint : transform;

    public int CharacterLevel { get; private set; } = 1;
    public bool IsInCombat { get; private set; } = false;
    public bool IsDead { get; private set; } = false;
    public bool IsImmune { get; private set; } = false;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => GetStatValue(StatType.Max_Health);

    public float CurrentMana { get; private set; }
    public float MaxMana => GetStatValue(StatType.Max_Mana);

    public ServerPlayerInventory Inventory { get; private set; }
    public ServerPlayerEquipment Equipment { get; private set; }
    public ServerPlayerSkills Skills { get; private set; }

    private readonly Dictionary<StatType, float> _currentStats = new Dictionary<StatType, float>();

    public event Action OnStatsChanged;
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action OnPlayerDied;

    private void Awake()
    {
        Inventory = new ServerPlayerInventory();
        Equipment = new ServerPlayerEquipment(this);
        Skills = gameObject.AddComponent<ServerPlayerSkills>();

        ResetStatsToDefault();
        RestoreFullResources();

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (IsDead) return;
        HandleRegeneration();
    }

    private void HandleRegeneration()
    {
        float hpRegen = GetStatValue(StatType.HP_Regen);
        if (hpRegen > 0f && CurrentHealth < MaxHealth)
        {
            ModifyHealth(hpRegen * Time.deltaTime);
        }

        float mpRegen = GetStatValue(StatType.MP_Regen);
        if (mpRegen > 0f && CurrentMana < MaxMana)
        {
            ModifyMana(mpRegen * Time.deltaTime);
        }
    }

    private void ResetStatsToDefault()
    {
        _currentStats[StatType.Strength] = 10f;
        _currentStats[StatType.Dexterity] = 10f;
        _currentStats[StatType.Intelligence] = 10f;

        _currentStats[StatType.Crit_Chance] = 5f;
        _currentStats[StatType.Crit_Damage] = 150f;
        _currentStats[StatType.Attack_Cast_Speed] = 0f;
        _currentStats[StatType.Fire_Damage_Percent_Bonus] = 0f;
        _currentStats[StatType.Lightning_Damage_Percent_Bonus] = 0f;
        _currentStats[StatType.Shadow_Damage_Percent_Bonus] = 0f;
        _currentStats[StatType.Physical_Damage_Percent_Bonus] = 0f;

        _currentStats[StatType.Armor] = 0f;
        _currentStats[StatType.Fire_Resistance] = 0f;
        _currentStats[StatType.Lightning_Resistance] = 0f;
        _currentStats[StatType.Shadow_Resistance] = 0f;

        _currentStats[StatType.Max_Health] = 100f + (CharacterLevel - 1) * 50f;
        _currentStats[StatType.HP_Regen] = 0f;

        _currentStats[StatType.Max_Mana] = 150f + (CharacterLevel - 1) * 15f;
        _currentStats[StatType.MP_Regen] = 3f;
    }

    public void RecalculateModifiersCache()
    {
        ResetStatsToDefault();

        var paperdoll = Equipment.GetPaperdoll();
        foreach (var pair in paperdoll)
        {
            InventorySlot slot = pair.Value;
            if (slot.IsEmpty || slot.itemData == null) continue;

            if (slot.itemData.staticStatsBlueprint != null)
            {
                foreach (var statMod in slot.itemData.staticStatsBlueprint)
                {
                    if (_currentStats.ContainsKey(statMod.statName))
                    {
                        _currentStats[statMod.statName] += statMod.baseValue;
                    }
                }
            }
        }

        _currentStats[StatType.Fire_Resistance] = Mathf.Clamp(_currentStats[StatType.Fire_Resistance], 0f, 75f);
        _currentStats[StatType.Lightning_Resistance] = Mathf.Clamp(_currentStats[StatType.Lightning_Resistance], 0f, 75f);
        _currentStats[StatType.Shadow_Resistance] = Mathf.Clamp(_currentStats[StatType.Shadow_Resistance], 0f, 75f);

        CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        CurrentMana = Mathf.Min(CurrentMana, MaxMana);

        OnStatsChanged?.Invoke();
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead || IsImmune) return;

        float finalDamage = DamageCalculator.CalculateFinalDamage(damageInfo, this, out bool isCrit);
        ModifyHealth(-finalDamage);

        Debug.Log($"[Combat] Игрок получил {finalDamage} урона {(isCrit ? "(КРИТ!)" : "")}. Осталось HP: {CurrentHealth}/{MaxHealth}");
    }

    public void ModifyHealth(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public bool TrySpendMana(float amount)
    {
        if (CurrentMana < amount) return false;
        ModifyMana(-amount);
        return true;
    }

    public void ModifyMana(float amount)
    {
        CurrentMana = Mathf.Clamp(CurrentMana + amount, 0f, MaxMana);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    public void RestoreFullResources()
    {
        IsDead = false;
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    private void Die()
    {
        IsDead = true;
        IsInCombat = false;
        Debug.Log("[Combat] Игрок погиб!");
        OnPlayerDied?.Invoke();
    }

    public float GetStatValue(StatType statType)
    {
        return _currentStats.TryGetValue(statType, out float val) ? val : 0f;
    }

    public void SendSystemMessage(string message)
    {
        Debug.Log($"[Server Network Message]: {message}");
    }
}