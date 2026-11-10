using ProjectCraft._Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectCraft._Server
{
    public class ServerNPCPortalKeeper : MonoBehaviour, IInteractable
    {
        [SerializeField] private float interactionRadius = 3.5f;
       
        // [SerializeField] private UIWindowType windowType; MAP
        public float InteractionRadius => interactionRadius;
        public Vector3 Position => transform.position;


        [Header("Scene Loading Settings")]
        [SerializeField] private string targetSceneName = "Instance_Prototype";

        public void Interact(ServerPlayerEntity player)
        {
            if (player == null) return;


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
    }
}