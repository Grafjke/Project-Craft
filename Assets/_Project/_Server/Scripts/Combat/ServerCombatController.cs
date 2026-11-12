using ProjectCraft._Shared;
using UnityEngine;

namespace ProjectCraft._Server
{
    [RequireComponent(typeof(ServerPlayerEntity))]
    [RequireComponent(typeof(ServerTargetController))]
    [RequireComponent(typeof(ServerMovementController))]
    public class ServerCombatController : MonoBehaviour
    {
        [Header("Default Unarmed Attack")]
        [SerializeField] private float unarmedDamage = 5f;
        [SerializeField] private float unarmedRange = 2.0f;
        [SerializeField] private float unarmedCooldown = 1.2f;

        [Header("Fallback Projectile (If Weapon Has None)")]
        [SerializeField] private GameObject defaultRangedProjectilePrefab;

        private ServerPlayerEntity _player;
        private ServerTargetController _targetController;
        private ServerMovementController _movementController;

        private float _autoAttackTimer;
        private bool _isAutoAttackActive;
        private bool _isChasingTarget;

        public bool IsAutoAttackActive => _isAutoAttackActive;

        private void Awake()
        {
            _player = GetComponent<ServerPlayerEntity>();
            _targetController = GetComponent<ServerTargetController>();
            _movementController = GetComponent<ServerMovementController>();
        }

        private void Update()
        {
            if (_player.IsDead) return;

            if (_autoAttackTimer > 0f)
            {
                _autoAttackTimer -= Time.deltaTime;
            }

            if (_targetController.currentTarget == null ||
                !_targetController.currentTarget.TryGetComponent<IDamageable>(out var target) ||
                target.IsDead)
            {
                if (_targetController.currentTarget != null)
                {
                    _targetController.ClearTarget();
                }
                StopAutoAttack();
                return;
            }
            if (_isAutoAttackActive)
            {
                ProcessCombatLoop(target);
            }
        }
        public void StartAutoAttackAndChase()
        {
            if (_player.IsDead) return;


            if (_targetController.currentTarget != null)
            {
                _movementController.StopMovement();

                _isAutoAttackActive = true;
                _isChasingTarget = true;
            }
        }
        public void OnManualMovementCommand()
        {
            _isChasingTarget = false;
        }
        public void StopAutoAttack()
        {
            if (_isAutoAttackActive || _isChasingTarget)
            {
                _isAutoAttackActive = false;
                _isChasingTarget = false;
            }
        }


        private void ProcessCombatLoop(IDamageable target)
        {
            UniversalItemData weapon = GetEquippedWeapon();
            float range = weapon != null ? weapon.attackRange : unarmedRange;
            float baseCooldown = weapon != null ? weapon.baseAttackCooldown : unarmedCooldown;

            float speedBonusPercent = _player.GetStatValue(StatType.Attack_Cast_Speed);
            float finalCooldown = baseCooldown / (1f + Mathf.Max(0f, speedBonusPercent) / 100f);

            float distance = Vector3.Distance(transform.position, target.Transform.position);

            if (distance > range)
            {
                if (_isChasingTarget)
                {
                    _movementController.MoveToTarget(target.Transform.position, range * 0.85f);
                }
                return;
            }

            if (_isChasingTarget)
            {
                _movementController.StopMovement();
                _isChasingTarget = false;
            }

            if (_movementController.IsMoving) return;

            Vector3 lookDir = (target.Transform.position - transform.position).normalized;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 15f);
            }

            if (_autoAttackTimer > 0f) return;

            _autoAttackTimer = finalCooldown;
            ExecuteAutoAttack(target, weapon);
        }


        private void ExecuteAutoAttack(IDamageable target, UniversalItemData weapon)
        {
            DamageInfo damageInfo = new DamageInfo
            {
                Attacker = _player,
                BaseDamage = weapon != null ? weapon.baseWeaponDamage : unarmedDamage,
                DamageType = weapon != null ? weapon.damageType : DamageType.Physical,
                ScalingAttribute = weapon != null ? weapon.scalingAttribute : ScalingAttribute.Strength,
                CanCrit = true
            };

            AttackType attackType = weapon != null ? weapon.attackType : AttackType.Melee;

            if (attackType == AttackType.Melee)
            {
                target.TakeDamage(damageInfo);
            }
            else
            {
                GameObject projPrefab = (weapon != null && weapon.projectilePrefab != null)
                    ? weapon.projectilePrefab
                    : defaultRangedProjectilePrefab;

                if (projPrefab != null)
                {
                    GameObject projObj = Instantiate(projPrefab, _player.ProjectileSpawnPoint.position, Quaternion.identity);
                    if (projObj.TryGetComponent<ServerHomingProjectile>(out var projectile))
                    {
                        projectile.Initialize(target, damageInfo);
                    }
                }
                else
                {
                    target.TakeDamage(damageInfo);
                }
            }
        }

        private UniversalItemData GetEquippedWeapon()
        {
            var paperdoll = _player.Equipment.GetPaperdoll();
            if (paperdoll.TryGetValue(7, out var mainHandSlot) && !mainHandSlot.IsEmpty)
            {
                return mainHandSlot.itemData;
            }
            return null;
        }
    }
}
