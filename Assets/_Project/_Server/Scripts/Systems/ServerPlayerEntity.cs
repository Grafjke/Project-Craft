using ProjectCraft._Server;
using ProjectCraft._Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerEntity : MonoBehaviour
{

    
    public int CharacterLevel { get; private set; } = 1;
    public bool IsInCombat { get; private set; } = false;

    
    public ServerPlayerInventory Inventory { get; private set; }
    public ServerPlayerEquipment Equipment { get; private set; }

    private readonly Dictionary<StatType, float> _currentStats = new Dictionary<StatType, float>();
    public event Action OnStatsChanged;

    private void Awake()
    {
        Inventory = new ServerPlayerInventory();
        Equipment = new ServerPlayerEquipment(this);
        ResetStatsToDefault();
    }
    private void ResetStatsToDefault()
    {
        _currentStats[StatType.Strength] = 10f;
        _currentStats[StatType.Dexterity] = 10f;
        _currentStats[StatType.Intelligence] = 10f;

        _currentStats[StatType.Crit_Chance] = 0f;
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

        _currentStats[StatType.Max_Mana] = 100f + (CharacterLevel - 1) * 15f;
        _currentStats[StatType.MP_Regen] = 0f;
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

        OnStatsChanged?.Invoke();
    }


    public float GetStatValue(StatType statType)
    {
        return _currentStats.TryGetValue(statType, out float val) ? val : 0f;
    }

    public void SendSystemMessage(string message)
    {
        Debug.LogWarning($"[Server Network Message]: {message}");
    }




}
