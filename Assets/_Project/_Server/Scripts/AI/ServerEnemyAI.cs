using ProjectCraft._Shared;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectCraft._Server
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ServerEnemyEntity))]
    public class ServerEnemyAI : MonoBehaviour
    {
        public enum AIState { Idle, Chasing, Returning }

        [Header("Current AI State")]
        [SerializeField] private AIState currentState = AIState.Idle;

        private NavMeshAgent _agent;
        private ServerEnemyEntity _entity;
        private EnemyData _data;
        private IDamageable _targetPlayer;
        private Vector3 _startPosition;
        private float _attackTimer;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _entity = GetComponent<ServerEnemyEntity>();
        }

        private void Start()
        {
            _data = _entity.Data;
            _startPosition = transform.position;

            if (_data == null)
            {
                Debug.LogError($"На объекте {name} отсутствует EnemyData в ServerEnemyEntity!");
                return;
            }

            _entity.OnDamageTaken += HandleDamageTaken;
            _entity.OnEnemyDied += HandleDeath;
        }

        private void OnDestroy()
        {
            if (_entity != null)
            {
                _entity.OnDamageTaken -= HandleDamageTaken;
                _entity.OnEnemyDied -= HandleDeath;
            }
        }

        private void Update()
        {
            if (_data == null || _entity.IsDead) return;

            if (_targetPlayer == null)
            {
                var player = FindFirstObjectByType<ServerPlayerEntity>();
                if (player != null) _targetPlayer = player;
                else return;
            }

            if (_attackTimer > 0f) _attackTimer -= Time.deltaTime;

            float distanceToPlayer = Vector3.Distance(transform.position, _targetPlayer.Transform.position);
            float distanceFromSpawn = Vector3.Distance(transform.position, _startPosition);

            switch (currentState)
            {
                case AIState.Idle:
                    if (!_targetPlayer.IsDead && distanceToPlayer <= _data.aggroRadius)
                    {
                        StartChasing();
                    }
                    break;

                case AIState.Chasing:
                    if (_targetPlayer.IsDead || distanceFromSpawn > _data.evadeRadius)
                    {
                        StartReturning();
                        return;
                    }

                    if (distanceToPlayer <= _data.attackRange)
                    {
                        if (_agent.isOnNavMesh) _agent.ResetPath();
                        LookAtTarget(_targetPlayer.Transform.position);
                        TryAttackPlayer();
                    }
                    else
                    {
                        if (_agent.isOnNavMesh)
                        {
                            _agent.SetDestination(_targetPlayer.Transform.position);
                        }
                    }
                    break;

                case AIState.Returning:
                    if (_agent.isOnNavMesh && !_agent.pathPending)
                    {
                        if (_agent.remainingDistance <= _agent.stoppingDistance + 0.3f)
                        {
                            ResetToIdle();
                        }
                    }
                    break;
            }
        }

        private void HandleDamageTaken(IDamageable attacker)
        {
            if (currentState == AIState.Idle && attacker != null && !attacker.IsDead)
            {
                _targetPlayer = attacker;
                StartChasing();
            }
        }

        private void TryAttackPlayer()
        {
            if (_attackTimer > 0f) return;

            _attackTimer = _data.attackCooldown;

            DamageInfo dmg = new DamageInfo
            {
                Attacker = _entity,
                BaseDamage = _data.baseDamage,
                DamageType = DamageType.Physical,
                ScalingAttribute = ScalingAttribute.None,
                CanCrit = false
            };

            _targetPlayer.TakeDamage(dmg);
        }

        private void LookAtTarget(Vector3 targetPos)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            dir.y = 0f;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
            }
        }

        private void StartChasing()
        {
            currentState = AIState.Chasing;
            _entity.IsImmune = false;
            _agent.speed = _data.chaseSpeed;
        }

        private void StartReturning()
        {
            currentState = AIState.Returning;
            _entity.IsImmune = true;
            _agent.speed = _data.returnSpeed;
            if (_agent.isOnNavMesh) _agent.SetDestination(_startPosition);
        }

        private void ResetToIdle()
        {
            currentState = AIState.Idle;
            if (_agent.isOnNavMesh) _agent.ResetPath();
            _entity.ResetEntity();
        }

        private void HandleDeath()
        {
            if (_agent.isOnNavMesh) _agent.ResetPath();
            _agent.enabled = false;
        }
    }
}
