using ProjectCraft._Client.UI;
using ProjectCraft._Server;
using ProjectCraft._Shared;
using UnityEngine;

public class UI_AltarWindowController : MonoBehaviour, IItemSlotClickHandler, IInteractableUIWindow
{


    [Header("Global References")]
    [SerializeField] private ServerPlayerEntity targetPlayer;

    [Header("Altar Slots UI")]
    [SerializeField] private UI_EquipmentSlot[] activeSlotsUI;  
    [SerializeField] private UI_EquipmentSlot[] passiveSlotsUI;

    private ServerPlayerSkills _subSkills;
    private ServerNPCAltar _activeAltar;

    private void Start()
    {
        EnsurePlayerConnection();
    }
    public void OnUIWindowOpened(MonoBehaviour npcSource)
    {
        if (npcSource is ServerNPCAltar altar)
        {
            InitializeInteraction(altar);
            RefreshWindow();
        }
    }

    private void OnEnable()
    {
        EnsurePlayerConnection();
        RefreshWindow();
    }

    private void Update()
    {
        if (_subSkills == null)
        {
            EnsurePlayerConnection();
            return;
        }
        if (_activeAltar != null && targetPlayer != null)
        {
            float distance = Vector3.Distance(targetPlayer.transform.position, _activeAltar.transform.position);

            if (distance > _activeAltar.InteractionRadius + 0.5f)
            {
                _activeAltar = null; 

                var windowManager = FindFirstObjectByType<UI_WindowManager>();
                if (windowManager != null)
                {
                    windowManager.CloseAllLeftWindows();
                }
            }
        }
    }

    public void InitializeInteraction(ServerNPCAltar altar)
    {
        _activeAltar = altar;
    }


    private void OnDestroy()
    {
        if (_subSkills != null)
            _subSkills.OnSkillBuildChanged -= RefreshWindow;
    }

    private void EnsurePlayerConnection()
    {
        if (targetPlayer == null)
            targetPlayer = FindFirstObjectByType<ServerPlayerEntity>();

        if (targetPlayer != null)
        {
            ServerPlayerSkills currentSkills = targetPlayer.GetComponent<ServerPlayerSkills>();
            if (currentSkills == null || _subSkills == currentSkills) return;

            // Отписываемся от старого компонента при перезапуске
            if (_subSkills != null) _subSkills.OnSkillBuildChanged -= RefreshWindow;

            _subSkills = currentSkills;
            _subSkills.OnSkillBuildChanged += RefreshWindow; // Реактивный паттерн

            InitializeSlots();
            RefreshWindow();
        }
    }


    private void InitializeSlots()
    {
        for (int i = 0; i < activeSlotsUI.Length; i++)
        {
            if (activeSlotsUI[i] != null) activeSlotsUI[i].SetupSlot(i, 0, this);
        }

        for (int i = 0; i < passiveSlotsUI.Length; i++)
        {
            if (passiveSlotsUI[i] != null) passiveSlotsUI[i].SetupSlot(i, 1, this);
        }
    }

    public void RefreshWindow()
    {
        if (_subSkills == null) return;

        InventorySlot[] activeData = _subSkills.GetActiveSlots();
        InventorySlot[] passiveData = _subSkills.GetPassiveSlots();

        for (int i = 0; i < activeSlotsUI.Length; i++)
        {
            if (activeSlotsUI[i] != null) activeSlotsUI[i].Render(activeData[i]);
        }

        for (int i = 0; i < passiveSlotsUI.Length; i++)
        {
            if (passiveSlotsUI[i] != null) passiveSlotsUI[i].Render(passiveData[i]);
        }
    }

    public void OnSlotRightClicked(int slotId, int customParam)
    {
        if (_subSkills == null || targetPlayer == null || targetPlayer.Inventory == null) return;

        bool isPassive = (customParam == 1);

        _subSkills.UnequipSkillFromBuild(slotId, isPassive, targetPlayer.Inventory);

        targetPlayer.Inventory.InvokeOnInventoryChanged(true);
    }

    public bool RequestEquipSkillToAltar(int invIndex, bool toPassive)
    {
        if (_subSkills == null || targetPlayer == null || targetPlayer.Inventory == null) return false;

        InventorySlot[] targetArray = toPassive ? _subSkills.GetPassiveSlots() : _subSkills.GetActiveSlots();
        int targetSlotId = -1;

        for (int i = 0; i < targetArray.Length; i++)
        {
            if (targetArray[i].IsEmpty)
            {
                targetSlotId = i;
                break;
            }
        }

        if (targetSlotId == -1) targetSlotId = 0;

        return _subSkills.EquipSkillToBuild(invIndex, targetSlotId, toPassive, targetPlayer.Inventory);
    }
}







