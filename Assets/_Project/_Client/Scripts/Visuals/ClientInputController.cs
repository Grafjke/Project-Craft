using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ClientInputController : MonoBehaviour
{
    private Camera _mainCamera;
    // Ссылка на компонент симуляции, который в будущем будет заменен отправкой пакета на сервер
    [SerializeField] private ServerMovementController serverMovement;

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
}



