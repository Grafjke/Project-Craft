using UnityEngine;
using ProjectCraft._Client.UI;

namespace ProjectCraft._Server
{
    public class ServerNPCBlacksmith : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 3.5f;
        [SerializeField] private GameObject craftWindowPanel;

        public void Interact(ServerPlayerEntity player)
        {
            if (player == null) return;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance > interactionRadius)
            {
                return;
            }

            var windowManager = FindFirstObjectByType<UI_WindowManager>();
            if (windowManager != null && craftWindowPanel != null)
            {
                var craftController = craftWindowPanel.GetComponent<UI_CraftWindowController>();

                windowManager.ToggleLeftWindowExclusive(craftWindowPanel);

                if (craftWindowPanel.activeInHierarchy)
                {
                    if (craftController != null)
                    {
                        craftController.InitializeInteraction(this);
                        craftController.ResetToFirstCascadeStep();
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        public float InteractionRadius => interactionRadius;
    }
}