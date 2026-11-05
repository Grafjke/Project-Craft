using ProjectCraft._Server;
using ProjectCraft._Shared;
using System;
using UnityEngine;

public class ServerPlayerSkills : MonoBehaviour
{
    public event Action OnSkillBuildChanged;

    private const int ACTIVE_SLOTS_COUNT = 4;
    private const int PASSIVE_SLOTS_COUNT = 4;

    private readonly InventorySlot[] _activeSlots = new InventorySlot[ACTIVE_SLOTS_COUNT];
    private readonly InventorySlot[] _passiveSlots = new InventorySlot[PASSIVE_SLOTS_COUNT];

    private ServerPlayerEntity _player;

    private void Awake()
    {
        _player = GetComponent<ServerPlayerEntity>();
        ClearBuild();
    }

    public void ClearBuild()
    {
        for (int i = 0; i < _activeSlots.Length; i++) _activeSlots[i] = default;
        for (int i = 0; i < _passiveSlots.Length; i++) _passiveSlots[i] = default;
    }
    public InventorySlot[] GetActiveSlots() => _activeSlots;
    public InventorySlot[] GetPassiveSlots() => _passiveSlots;



    public bool EquipSkillToBuild(int invIndex, int targetSlotId, bool toPassiveTab, ServerPlayerInventory inventory)
    {
        InventorySlot invSlot = inventory.GetSlot(invIndex, true);
        if (invSlot.IsEmpty || invSlot.skillData == null) return false;

        if (invSlot.skillData.isPassive != toPassiveTab)
        {
            _player.SendSystemMessage("Нельзя вставить активный навык в пассивный слот и наоборот!");
            return false;
        }

        if (IsSkillAlreadyEquipped(invSlot.skillData.skillID))
        {
            _player.SendSystemMessage("Этот навык уже экипирован в вашем билде!");
            return false;
        }

        var targetArray = toPassiveTab ? _passiveSlots : _activeSlots;
        if (targetSlotId < 0 || targetSlotId >= targetArray.Length) return false;

        InventorySlot previouslyEquipped = targetArray[targetSlotId];

        targetArray[targetSlotId] = invSlot;

        if (!previouslyEquipped.IsEmpty)
        {
            inventory.SetSlot(invIndex, previouslyEquipped, true);
        }
        else
        {
            inventory.ClearSlot(invIndex, true);
        }

        ValidateSkillsWeaponCompliance();

        OnSkillBuildChanged?.Invoke();
        inventory.InvokeOnInventoryChanged(true);
        return true;
    }

    public void UnequipSkillFromBuild(int slotId, bool isPassiveTab, ServerPlayerInventory inventory)
    {
        var targetArray = isPassiveTab ? _passiveSlots : _activeSlots;
        if (slotId < 0 || slotId >= targetArray.Length || targetArray[slotId].IsEmpty) return;

        if (!inventory.HasEnoughFreeSlots(true, 1))
        {
            _player.SendSystemMessage("Нет места во вкладке навыков!");
            return;
        }

        InventorySlot skillSlot = targetArray[slotId];
        targetArray[slotId] = default;

        inventory.AddSkill(skillSlot.skillData);

        OnSkillBuildChanged?.Invoke();
    }

    private bool IsSkillAlreadyEquipped(string skillId)
    {
        foreach (var slot in _activeSlots) if (!slot.IsEmpty && slot.skillData != null && slot.skillData.skillID == skillId) return true;
        foreach (var slot in _passiveSlots) if (!slot.IsEmpty && slot.skillData != null && slot.skillData.skillID == skillId) return true;
        return false;
    }


    public void ValidateSkillsWeaponCompliance()
    {
        if (_player == null || _player.Equipment == null) return;

        InventorySlot weaponSlot = _player.Equipment.GetPaperdoll()[7];
        string currentWeaponTag = (weaponSlot.IsEmpty || weaponSlot.itemData == null) ? "Bare_Hands" : weaponSlot.itemData.weaponOrArmorTag;

        // Тут будет отключени интерактивности скила если теги оружия не подходят
    }
}



