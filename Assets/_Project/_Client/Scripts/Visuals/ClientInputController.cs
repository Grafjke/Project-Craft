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

        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (serverTarget != null)
            {
                serverTarget.SelectTargetViaTab();
            }
        }
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteractWithNearestNPC();
        }
    }
    private void TryInteractWithNearestNPC()
    {
        if (_localPlayer == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        ServerNPCAltar closestAltar = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            ServerNPCAltar altar = hitCollider.GetComponent<ServerNPCAltar>();
            if (altar != null)
            {
                float distance = Vector3.Distance(transform.position, altar.transform.position);

                if (distance <= altar.InteractionRadius && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestAltar = altar;
                }
            }
        }

        if (closestAltar != null)
        {
            closestAltar.Interact(_localPlayer);
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



