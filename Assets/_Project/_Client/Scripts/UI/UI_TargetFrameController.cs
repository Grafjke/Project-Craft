using ProjectCraft._Server;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCraft._Client
{
    public class UI_TargetFrameController : MonoBehaviour
    {
        [Header("Frame Root")]
        [SerializeField] private GameObject frameRoot;

        [Header("Target Info UI")]
        [SerializeField] private TextMeshProUGUI targetNameText;
        [SerializeField] private Image hpFillImage;
        [SerializeField] private TextMeshProUGUI hpText;

        [Header("Statuses Container")]
        [SerializeField] private Transform statusIconsContainer;

        private ServerTargetController _targetController;
        private ServerEnemyEntity _currentEnemy;

        private void Start()
        {
            if (frameRoot != null)
            {
                frameRoot.SetActive(false);
            }

            _targetController = FindFirstObjectByType<ServerTargetController>();
            if (_targetController != null)
            {
                _targetController.OnTargetChanged += HandleTargetChanged;
                HandleTargetChanged(_targetController.currentTarget);
            }
        }

        private void OnDestroy()
        {
            if (_targetController != null)
            {
                _targetController.OnTargetChanged -= HandleTargetChanged;
            }
            UnsubscribeFromCurrentEnemy();
        }

        private void HandleTargetChanged(Transform newTarget)
        {
            UnsubscribeFromCurrentEnemy();

            if (newTarget == null || !newTarget.TryGetComponent<ServerEnemyEntity>(out var enemy) || enemy.IsDead)
            {
                if (frameRoot != null) frameRoot.SetActive(false);
                return;
            }

            _currentEnemy = enemy;
            _currentEnemy.OnHealthChanged += UpdateTargetHealth;
            _currentEnemy.OnEnemyDied += HandleEnemyDied;

            if (frameRoot != null) frameRoot.SetActive(true);
            if (targetNameText != null) targetNameText.text = _currentEnemy.EntityName;

            UpdateTargetHealth(_currentEnemy.CurrentHealth, _currentEnemy.MaxHealth);
        }

        private void UpdateTargetHealth(float current, float max)
        {
            if (hpFillImage != null)
            {
                hpFillImage.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            }

            if (hpText != null)
            {
                hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
        }

        private void HandleEnemyDied()
        {
            UnsubscribeFromCurrentEnemy();
            if (frameRoot != null) frameRoot.SetActive(false);
        }

        private void UnsubscribeFromCurrentEnemy()
        {
            if (_currentEnemy != null)
            {
                _currentEnemy.OnHealthChanged -= UpdateTargetHealth;
                _currentEnemy.OnEnemyDied -= HandleEnemyDied;
                _currentEnemy = null;
            }
        }
    }
}
