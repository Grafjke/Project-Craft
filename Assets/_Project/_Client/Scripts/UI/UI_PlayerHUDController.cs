using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectCraft._Client
{
    public class UI_PlayerHUDController : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Image hpFillImage;
        [SerializeField] private TextMeshProUGUI hpText;

        [Header("Mana UI")]
        [SerializeField] private Image mpFillImage;
        [SerializeField] private TextMeshProUGUI mpText;

        private ServerPlayerEntity _player;

        private void Start()
        {
            _player = FindFirstObjectByType<ServerPlayerEntity>();
            if (_player == null) return;

            _player.OnHealthChanged += UpdateHealthUI;
            _player.OnManaChanged += UpdateManaUI;

            UpdateHealthUI(_player.CurrentHealth, _player.MaxHealth);
            UpdateManaUI(_player.CurrentMana, _player.MaxMana);
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.OnHealthChanged -= UpdateHealthUI;
                _player.OnManaChanged -= UpdateManaUI;
            }
        }

        private void UpdateHealthUI(float current, float max)
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

        private void UpdateManaUI(float current, float max)
        {
            if (mpFillImage != null)
            {
                mpFillImage.fillAmount = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            }

            if (mpText != null)
            {
                mpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
        }
    }
}
