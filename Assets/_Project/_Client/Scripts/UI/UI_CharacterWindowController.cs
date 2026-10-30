using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ProjectCraft._Server;
using ProjectCraft._Shared;

namespace ProjectCraft._Client.UI
{
    public class UI_CharacterWindowController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ServerPlayerEntity targetPlayer;

        [Header("Paperdoll Slots UI (Должно быть ровно 9 слотов в инспекторе)")]
        [SerializeField] private UI_EquipmentSlot[] equipmentSlots;

        [Header("Stats Panel UI")]
        [SerializeField] private TextMeshProUGUI statsListText; // Единое текстовое поле с поддержкой Rich Text

        private ServerPlayerEquipment _subEquipment;
        private ServerPlayerEntity _subEntity;

        private void Start()
        {
            if (targetPlayer == null)
            {
                targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();
            }
            InitializeWindow();
        }

        private void OnEnable()
        {
            InitializeWindow();
            RefreshWindow();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void InitializeWindow()
        {
            if (targetPlayer == null) return;

            for (int i = 0; i < equipmentSlots.Length; i++)
            {
                if (equipmentSlots[i] != null)
                {
                    equipmentSlots[i].SetupSlot(i, (SlotType)i, this);
                }
            }

            if (_subEntity != targetPlayer)
            {
                Unsubscribe();

                _subEntity = targetPlayer;
                _subEquipment = targetPlayer.Equipment;

                if (_subEquipment != null) _subEquipment.OnEquipmentChanged += HandleEquipmentChanged;
                if (_subEntity != null) _subEntity.OnStatsChanged += HandleStatsChanged;
            }
        }

        private void Unsubscribe()
        {
            if (_subEquipment != null) _subEquipment.OnEquipmentChanged -= HandleEquipmentChanged;
            if (_subEntity != null) _subEntity.OnStatsChanged -= HandleStatsChanged;
        }

        private void HandleEquipmentChanged(int slotId)
        {
            if (slotId >= 0 && slotId < equipmentSlots.Length)
            {
                var paperdoll = _subEquipment.GetPaperdoll();
                equipmentSlots[slotId].Render(paperdoll[slotId]);
            }
        }

        private void HandleStatsChanged()
        {
            RefreshStatsUI();
        }

        public void RefreshWindow()
        {
            if (targetPlayer == null || _subEquipment == null) return;

            var paperdoll = _subEquipment.GetPaperdoll();
            for (int i = 0; i < equipmentSlots.Length; i++)
            {
                if (paperdoll.TryGetValue(i, out InventorySlot slotData))
                {
                    equipmentSlots[i].Render(slotData);
                }
            }

            RefreshStatsUI();
        }

        private void RefreshStatsUI()
        {
            if (_subEntity == null) return;

            float armor = _subEntity.GetStatValue(StatType.Armor);
            float physAbsorption = (armor + 500f) > 0 ? (armor / (armor + 500f)) * 100f : 0f;
            if (physAbsorption > 75f) physAbsorption = 75f;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"УРОВЕНЬ: {_subEntity.CharacterLevel}\n");

            // Блок 1
            sb.AppendLine("<b>[БАЗОВЫЕ АТРИБУТЫ]</b>");
            sb.AppendLine($"Сила (STR): {_subEntity.GetStatValue(StatType.Strength)}");
            sb.AppendLine($"Ловкость (DEX): {_subEntity.GetStatValue(StatType.Dexterity)}");
            sb.AppendLine($"Интеллект (INT): {_subEntity.GetStatValue(StatType.Intelligence)}\n");

            // Блок 2
            sb.AppendLine("<b>[АТАКА]</b>");
            sb.AppendLine($"Крит. Шанс: {_subEntity.GetStatValue(StatType.Crit_Chance)}%");
            sb.AppendLine($"Крит. Урон: {_subEntity.GetStatValue(StatType.Crit_Damage)}%");
            sb.AppendLine($"Скорость атаки/каста: +{_subEntity.GetStatValue(StatType.Attack_Cast_Speed)}");
            sb.AppendLine($"Урон Огня: +{_subEntity.GetStatValue(StatType.Fire_Damage_Percent_Bonus)}");
            sb.AppendLine($"Урон Молнии: +{_subEntity.GetStatValue(StatType.Lightning_Damage_Percent_Bonus)}");
            sb.AppendLine($"Урон Тьмы: +{_subEntity.GetStatValue(StatType.Shadow_Damage_Percent_Bonus)}");
            sb.AppendLine($"Физический урон: +{_subEntity.GetStatValue(StatType.Physical_Damage_Percent_Bonus)}\n");

            // Блок 3
            sb.AppendLine("<b>[ЗАЩИТА]</b>");
            sb.AppendLine($"Броня: {armor} ({physAbsorption:F1}%)");
            sb.AppendLine($"Сопротивление Огню: {_subEntity.GetStatValue(StatType.Fire_Resistance)}%");
            sb.AppendLine($"Сопротивление Молнии: {_subEntity.GetStatValue(StatType.Lightning_Resistance)}%");
            sb.AppendLine($"Сопротивление Тьме: {_subEntity.GetStatValue(StatType.Shadow_Resistance)}%\n");

            // Блок 4
            sb.AppendLine("<b>[ВОССТАНОВЛЕНИЕ]</b>");
            sb.AppendLine($"Макс. HP: {_subEntity.GetStatValue(StatType.Max_Health)}");
            sb.AppendLine($"Регенерация HP/сек: {_subEntity.GetStatValue(StatType.HP_Regen)}");
            sb.AppendLine($"Макс. MP: {_subEntity.GetStatValue(StatType.Max_Mana)}");
            sb.AppendLine($"Регенерация MP/сек: {_subEntity.GetStatValue(StatType.MP_Regen)}");

            statsListText.text = sb.ToString();
        }

        public void RequestUnequip(int slotId)
        {
            if (_subEquipment != null && targetPlayer.Inventory != null)
            {
                _subEquipment.UnequipItem(slotId, targetPlayer.Inventory);
            }
        }
    }
}