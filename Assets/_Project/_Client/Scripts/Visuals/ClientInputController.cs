using ProjectCraft._Server;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClientInputController : MonoBehaviour
{
    private Camera _mainCamera;
    // Ссылка на компонент симуляции, который в будущем будет заменен отправкой пакета на сервер
    [Header("Server System References")]
    [SerializeField] private ServerMovementController serverMovement;
    [SerializeField] private ServerTargetController serverTarget;
    [SerializeField] private ServerCombatController serverCombat;

    [Header("Interaction Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask npcLayer;

    private ServerPlayerEntity _localPlayer;

    

    void Start()
    {
        _localPlayer = GetComponent<ServerPlayerEntity>();
        // Больше не кэшируем камеру жестко в Start, так как она уничтожится при смене сцены
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
        
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (serverCombat != null)
            {
                serverCombat.StartAutoAttackAndChase();
            }
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (serverTarget != null) serverTarget.ClearTarget();
            if (serverCombat != null) serverCombat.StopAutoAttack();
        }
    }
    private void TryInteractWithNearestNPC()
    {
        if (_localPlayer == null) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

        IInteractable closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {

            var interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float distance = Vector3.Distance(transform.position, interactable.Position);


            if (distance <= interactable.InteractionRadius && distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        if (closestInteractable != null)
        {
            closestInteractable.Interact(_localPlayer);
        }
    }

    private void SendMovementRequest()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

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
        if (serverCombat != null) serverCombat.OnManualMovementCommand();
    }

    private void TrySelectTargetUnderCursor()
    {

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }


        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            var damageable = hit.transform.GetComponentInParent<IDamageable>();
            if (damageable != null && damageable.Transform.CompareTag("Enemy"))
            {
                if (serverTarget != null) serverTarget.SetTarget(damageable.Transform);
            }
            else
            {
                if (serverTarget != null) serverTarget.ClearTarget();
            }
        }
    }
}



