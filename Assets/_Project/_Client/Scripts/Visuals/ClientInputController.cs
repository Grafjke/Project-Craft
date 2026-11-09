using ProjectCraft._Server;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Unity.Multiplayer.Editor.EditorMultiplayerRolesManager.AutomaticSelection;

public class ClientInputController : MonoBehaviour
{
    private Camera _mainCamera;
    // Ссылка на компонент симуляции, который в будущем будет заменен отправкой пакета на сервер
    [Header("Server System References")]
    [SerializeField] private ServerMovementController serverMovement;
    [SerializeField] private ServerTargetController serverTarget;

    [Header("Interaction Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask npcLayer;

    private ServerPlayerEntity _localPlayer;

    

    void Start()
    {
        _localPlayer = GetComponent<ServerPlayerEntity>();
        _mainCamera = Camera.main;
    }

    void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (serverTarget != null) serverTarget.SelectTargetViaTab();
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteractWithNearestNPC();
        }

        if (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame)
        {
            var windowManager = FindFirstObjectByType<UI_WindowManager>();

            if (windowManager != null && windowManager.characterWindow != null)
            {
                windowManager.ToggleLeftWindowExclusive(windowManager.characterWindow);

            }
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            SendMovementRequest();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TrySelectTargetUnderCursor();
        }
    }
    private void TryInteractWithNearestNPC()
    {
        if (_localPlayer == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        MonoBehaviour closestInteractable = null;
        float closestDistance = Mathf.Infinity;
        string npcType = "";

        foreach (var hitCollider in hitColliders)
        {
            ServerNPCAltar altar = hitCollider.GetComponent<ServerNPCAltar>();
            if (altar != null)
            {
                float distance = Vector3.Distance(transform.position, altar.transform.position);
                if (distance <= altar.InteractionRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = altar;
                    npcType = "Altar";
                }
                continue;
            }

            ServerNPCBlacksmith blacksmith = hitCollider.GetComponent<ServerNPCBlacksmith>();
            if (blacksmith != null)
            {
                float distance = Vector3.Distance(transform.position, blacksmith.transform.position);
                if (distance <= blacksmith.InteractionRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = blacksmith;
                    npcType = "Blacksmith";
                }
            }
        }

        if (closestInteractable != null)
        {
            if (npcType == "Altar" && closestInteractable is ServerNPCAltar targetAltar)
            {
                targetAltar.Interact(_localPlayer);
            }
            else if (npcType == "Blacksmith" && closestInteractable is ServerNPCBlacksmith targetBlacksmith)
            {
                targetBlacksmith.Interact(_localPlayer);
            }
        }
    }

    private void SendMovementRequest()
    {

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            if (serverMovement != null)
            {
                serverMovement.OnClientMovementRequest(hit.point);
            }
        }
    }

    private void TrySelectTargetUnderCursor()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            if (hit.transform.CompareTag("Enemy"))
            {
                if (serverTarget != null) serverTarget.SetTarget(hit.transform);
            }
            else
            {
                if (serverTarget != null) serverTarget.ClearTarget();
            }
        }
    }
}



