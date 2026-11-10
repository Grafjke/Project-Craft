using UnityEngine;
using ProjectCraft._Client.UI;
using ProjectCraft._Shared;

namespace ProjectCraft._Server
{
    public class ServerNPCBlacksmith : MonoBehaviour, IInteractable
    {
        [SerializeField] private float interactionRadius = 3.5f;
        [SerializeField] private UIWindowType windowType = UIWindowType.Craft;

        public float InteractionRadius => interactionRadius;
        public Vector3 Position => transform.position;

        public void Interact(ServerPlayerEntity player)
        {
            if (UI_WindowManager.Instance != null)
            {
                GameObject targetWindow = UI_WindowManager.Instance.GetWindowByType(windowType);
                UI_WindowManager.Instance.OpenWindowForNPC(targetWindow, this);
            }
        }
    }
}