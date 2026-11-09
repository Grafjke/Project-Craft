using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectCraft._Server
{
    public class ServerNPCPortalKeeper : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 3.5f;

        [Header("Scene Loading Settings")]
        [SerializeField] private string targetSceneName = "Instance_Prototype";

        public void Interact(ServerPlayerEntity player)
        {
            if (player == null) return;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance > interactionRadius)
            {
                player.SendSystemMessage("Вы слишком далеко от Хранителя порталов!");
                return;
            }

            Debug.Log($"[PortalKeeper] Игрок {player.name} активировал портал. Загрузка сцены: {targetSceneName}");

            // 2. В будущем здесь будет вызов UI-окна с картой.
            // Сейчас — мгновенная загрузка боевого инстанса.
            // Используем Single-загрузку, чтобы полностью сменить локацию.
            SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta; // Пурпурный цвет для порталов
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        public float InteractionRadius => interactionRadius;
    }
}