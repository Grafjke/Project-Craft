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

    private void Awake()
    {
     
        Inventory = new ServerPlayerInventory();
        Equipment = new ServerPlayerEquipment(this);
    }

    public void RecalculateModifiersCache()
    {
        // Сюда мы будем обращаться, когда Equipment скажет "шмот изменился!"
        // Менеджер статов соберет данные напрямую из Equipment.GetPaperdoll()
    }

    public void SendSystemMessage(string message)
    {
        Debug.LogWarning($"[Server Network Message]: {message}");
    }




}
