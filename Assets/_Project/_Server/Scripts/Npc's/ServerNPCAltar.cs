using UnityEngine;
using ProjectCraft._Client.UI;

namespace ProjectCraft._Server
{
    public class ServerNPCAltar : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 3.5f;
        [SerializeField] private GameObject altarWindowPanel;

        public void Interact(ServerPlayerEntity player)
        {
            if (player == null) return;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance > interactionRadius)
            {
                player.SendSystemMessage("Вы слишком далеко от Алтаря Душ!");
                return;
            }

            var windowManager = FindFirstObjectByType<UI_WindowManager>();
            if (windowManager != null && altarWindowPanel != null)
            {
                windowManager.ToggleLeftWindowExclusive(altarWindowPanel);

                var altarController = altarWindowPanel.GetComponent<UI_AltarWindowController>();
                if (altarController != null && altarWindowPanel.activeInHierarchy)
                {
                    // Передаем ссылку на этот конкретный алтарь для трекинга дистанции
                    altarController.InitializeInteraction(this);
                    altarController.RefreshWindow();
                }

                Debug.Log($"Игрок {player.name} успешно взаимодействовал с Алтарем.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        public float InteractionRadius => interactionRadius;
    }
}
