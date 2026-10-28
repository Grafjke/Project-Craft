using ProjectCraft._Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerEntity : MonoBehaviour
{
    public event Action<int, int> OnHealthChanged;

    [Header("Character Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Header("Core Attributes (MVP)")]
    public int strength = 10;
    public int intellect = 10;
    public int armor = 0;

    [Header("Inventory & Equipment Systems (Server Side)")]
    [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>(20);

    private Dictionary<SlotID, InventorySlot> equipmentDoll = new Dictionary<SlotID, InventorySlot>();

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        InitInventoryAndDoll();
    }

    private void InitInventoryAndDoll()
    {
       
        for (int i = 0; i < 20; i++)
        {
            inventorySlots.Add(new InventorySlot(null, 0));
        }

        
        foreach (SlotID slot in Enum.GetValues(typeof(SlotID)))
        {
            if (slot != SlotID.None)
            {
                equipmentDoll[slot] = new InventorySlot(null, 0);
            }
        }
    }

    
    public void ModifyHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }


    public bool EquipItem(UniversalItemData equipment)
    {
        if (equipment == null || equipment.itemType != ItemType.Equipment) return false;

        // Серверная валидация: проверяем требования по уровню из GDD
        // (Задел на будущее)

        equipmentDoll[equipment.slotID] = new InventorySlot(equipment, 1);
        Debug.Log($"Экипирован предмет: {equipment.itemName} в слот {equipment.slotID}");
        return true;
    }








}
