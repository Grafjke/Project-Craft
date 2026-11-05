using UnityEngine;

    public class ServerNPCAltar : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 3.5f;

        private Transform _playerTransform;
        private bool _isPlayerNear = false;

        private void Start()
        {
            var movementController = FindFirstObjectByType<ServerMovementController>();
            if (movementController != null) _playerTransform = movementController.transform;
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            float distance = Vector3.Distance(transform.position, _playerTransform.position);

            if (distance <= interactionRadius && !_isPlayerNear)
            {
                _isPlayerNear = true;
                // На стадии MVP шлём симуляцию открытия окна на клиент
                Debug.Log("[NPC Altar] Игрок подошёл к алтарю! Открываем UI Алтаря Знаний.");
                // В будущем: UI_WindowManager.Instance.OpenAltarWindow();
            }
            else if (distance > interactionRadius && _isPlayerNear)
            {
                _isPlayerNear = false;
                Debug.Log("[NPC Altar] Игрок отошёл. Принудительно закрываем интерфейс алтаря.");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }

