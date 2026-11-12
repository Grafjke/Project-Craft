using ProjectCraft._Server;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCraft._Client
{
    public class UI_WorldHeadbarsController : MonoBehaviour
    {
        [Header("Player Headbar")]
        [SerializeField] private RectTransform playerHeadbarRoot;
        [SerializeField] private Image playerHpFill;
        [SerializeField] private Image playerMpFill;
        [SerializeField] private Vector3 playerOffset = new Vector3(0f, 2.3f, 0f);

        [Header("Target Enemy Headbar")]
        [SerializeField] private RectTransform enemyHeadbarRoot;
        [SerializeField] private Image enemyHpFill;
        [SerializeField] private Vector3 enemyOffset = new Vector3(0f, 2.3f, 0f);

        private ServerPlayerEntity _player;
        private ServerTargetController _targetController;
        private ServerEnemyEntity _currentEnemy;
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;

            if (enemyHeadbarRoot != null)
            {
                enemyHeadbarRoot.gameObject.SetActive(false);
            }

            _player = FindAnyObjectByType<ServerPlayerEntity>();
            if (_player != null)
            {
                _player.OnHealthChanged += UpdatePlayerHp;
                _player.OnManaChanged += UpdatePlayerMp;
                UpdatePlayerHp(_player.CurrentHealth, _player.MaxHealth);
                UpdatePlayerMp(_player.CurrentMana, _player.MaxMana);
            }

            _targetController = FindAnyObjectByType<ServerTargetController>();
            if (_targetController != null)
            {
                _targetController.OnTargetChanged += HandleTargetChanged;
                HandleTargetChanged(_targetController.currentTarget);
            }
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.OnHealthChanged -= UpdatePlayerHp;
                _player.OnManaChanged -= UpdatePlayerMp;
            }

            if (_targetController != null)
            {
                _targetController.OnTargetChanged -= HandleTargetChanged;
            }

            UnsubscribeEnemy();
        }

        private void LateUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            if (_player != null && !_player.IsDead && playerHeadbarRoot != null)
            {
                UpdateScreenPosition(playerHeadbarRoot, _player.transform.position + playerOffset);
            }
            else if (playerHeadbarRoot != null && playerHeadbarRoot.gameObject.activeSelf)
            {
                playerHeadbarRoot.gameObject.SetActive(false);
            }

            if (_currentEnemy != null && !_currentEnemy.IsDead && enemyHeadbarRoot != null)
            {
                UpdateScreenPosition(enemyHeadbarRoot, _currentEnemy.transform.position + enemyOffset);
            }
        }

        private void UpdateScreenPosition(RectTransform barTransform, Vector3 worldPos)
        {
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0f)
            {
                if (barTransform.gameObject.activeSelf) barTransform.gameObject.SetActive(false);
                return;
            }

            if (!barTransform.gameObject.activeSelf) barTransform.gameObject.SetActive(true);
            barTransform.position = screenPos;
        }

        private void HandleTargetChanged(Transform newTarget)
        {
            UnsubscribeEnemy();

            if (newTarget == null || !newTarget.TryGetComponent<ServerEnemyEntity>(out var enemy) || enemy.IsDead)
            {
                if (enemyHeadbarRoot != null) enemyHeadbarRoot.gameObject.SetActive(false);
                return;
            }

            _currentEnemy = enemy;
            _currentEnemy.OnHealthChanged += UpdateEnemyHp;
            _currentEnemy.OnEnemyDied += HandleEnemyDied;

            UpdateEnemyHp(_currentEnemy.CurrentHealth, _currentEnemy.MaxHealth);
            if (enemyHeadbarRoot != null) enemyHeadbarRoot.gameObject.SetActive(true);
        }

        private void UpdatePlayerHp(float current, float max)
        {
            if (playerHpFill != null) playerHpFill.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        }

        private void UpdatePlayerMp(float current, float max)
        {
            if (playerMpFill != null) playerMpFill.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        }

        private void UpdateEnemyHp(float current, float max)
        {
            if (enemyHpFill != null) enemyHpFill.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        }

        private void HandleEnemyDied()
        {
            UnsubscribeEnemy();
            if (enemyHeadbarRoot != null) enemyHeadbarRoot.gameObject.SetActive(false);
        }

        private void UnsubscribeEnemy()
        {
            if (_currentEnemy != null)
            {
                _currentEnemy.OnHealthChanged -= UpdateEnemyHp;
                _currentEnemy.OnEnemyDied -= HandleEnemyDied;
                _currentEnemy = null;
            }
        }
    }
}
