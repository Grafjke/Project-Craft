using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static Unity.Multiplayer.Editor.EditorMultiplayerRolesManager.AutomaticSelection;

public class ClientInputController : MonoBehaviour
{
    private Camera _mainCamera;
    // Ссылка на компонент симуляции, который в будущем будет заменен отправкой пакета на сервер
    [Header("Server System References")]
    [SerializeField] private ServerMovementController serverMovement;
    [SerializeField] private ServerTargetController serverTarget;




    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {

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



