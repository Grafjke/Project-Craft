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
                player.SendSystemMessage("Вы слишком далеко от Кузнеца!");
                return;
            }

            var windowManager = FindFirstObjectByType<UI_WindowManager>();
            if (windowManager != null && craftWindowPanel != null)
            {
                windowManager.ToggleLeftWindowExclusive(craftWindowPanel);

                var craftController = craftWindowPanel.GetComponent<UI_CraftWindowController>();
                if (craftController != null && craftWindowPanel.activeInHierarchy)
                {

                    craftController.ResetToFirstCascadeStep();

                }

                Debug.Log($"Игрок {player.name} успешно взаимодействовал с Кузнецом.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.orange; // Оранжевый цвет гизов для Кузнеца
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        public float InteractionRadius => interactionRadius;
    }
}