using UnityEngine;
using UnityEngine.UI;
using ProjectCraft._Server;
using ProjectCraft._Shared;

namespace ProjectCraft._Client.UI
{
    public class UI_HUDActionBar : MonoBehaviour
    {
        [Header("HUD Slots")]
        [SerializeField] private Image[] activeSkillImages;
        [SerializeField] private Image[] cooldownOverlays;
        [SerializeField] private TMPro.TextMeshProUGUI[] cooldownTexts;

        private ServerPlayerSkills _serverSkills;
        private bool _isSubscribed = false;

        private void Start()
        {
            TryConnectToPlayer();
        }

        private void Update()
        {
            if (_serverSkills == null)
            {
                TryConnectToPlayer();
                return;
            }

            UpdateCooldownsVisuals();
        }

        private void TryConnectToPlayer()
        {
            var playerEntity = FindFirstObjectByType<ServerPlayerEntity>();
            if (playerEntity != null)
            {
                _serverSkills = playerEntity.GetComponent<ServerPlayerSkills>();

                if (_serverSkills != null && !_isSubscribed)
                {

                    _serverSkills.OnSkillBuildChanged += UpdateHUD;
                    _isSubscribed = true;
                    UpdateHUD();
                }
            }
        }

        private void OnDestroy()
        {
            if (_serverSkills != null && _isSubscribed)
            {
                _serverSkills.OnSkillBuildChanged -= UpdateHUD;
            }
        }

        private void UpdateHUD()
        {
            if (_serverSkills == null) return;

            InventorySlot[] actives = _serverSkills.GetActiveSlots();

            for (int i = 0; i < activeSkillImages.Length; i++)
            {
                if (i >= actives.Length) break;
                if (activeSkillImages[i] == null) continue;

                InventorySlot slot = actives[i];

                if (slot.IsEmpty || slot.skillData == null)
                {
                    activeSkillImages[i].enabled = false;
                    activeSkillImages[i].sprite = null;

                    if (cooldownOverlays != null && i < cooldownOverlays.Length && cooldownOverlays[i] != null)
                        cooldownOverlays[i].gameObject.SetActive(false);

                    if (cooldownTexts != null && i < cooldownTexts.Length && cooldownTexts[i] != null)
                        cooldownTexts[i].text = "";
                }
                else
                {
                    activeSkillImages[i].enabled = true;
                    activeSkillImages[i].sprite = slot.skillData.skillIcon;
                }
            }
        }
        private void UpdateCooldownsVisuals()
        {
            InventorySlot[] actives = _serverSkills.GetActiveSlots();

            for (int i = 0; i < activeSkillImages.Length; i++)
            {
                if (i >= actives.Length) break;

                InventorySlot slot = actives[i];
                if (slot.IsEmpty || slot.skillData == null) continue;


                bool isOnCooldown = false;

                if (isOnCooldown)
                {
                    if (cooldownOverlays != null && i < cooldownOverlays.Length && cooldownOverlays[i] != null)
                    {
                        cooldownOverlays[i].gameObject.SetActive(true);

                    }

                    if (cooldownTexts != null && i < cooldownTexts.Length && cooldownTexts[i] != null)
                    {

                    }
                }
                else
                {
                    if (cooldownOverlays != null && i < cooldownOverlays.Length && cooldownOverlays[i] != null)
                        cooldownOverlays[i].gameObject.SetActive(false);

                    if (cooldownTexts != null && i < cooldownTexts.Length && cooldownTexts[i] != null)
                        cooldownTexts[i].text = "";
                }
            }
        }
    }
}
