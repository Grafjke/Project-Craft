using ProjectCraft._Shared;
using UnityEngine;
using UnityEngine.AI;


namespace ProjectCraft._Server
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ServerEnemyAI : MonoBehaviour
    {
        public enum AIState { Idle, Chasing, Returning }

        [Header("Linked Data Blueprint")]
        [SerializeField] private EnemyData enemyData;

        [Header("Current AI State")]
        [SerializeField] private AIState currentState = AIState.Idle;

        private NavMeshAgent _agent;
        private Transform _playerTransform;
        private Vector3 _startPosition;

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _startPosition = transform.position;

            var player = FindObjectOfType<ServerMovementController>();
            if (player != null) _playerTransform = player.transform;

            if (enemyData == null)
            {
                Debug.LogError($"На объекте {name} отсутствует EnemyData!");
            }
        }

        void Update()
        {
            if (_playerTransform == null || enemyData == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            switch (currentState)
            {
                case AIState.Idle:
                    if (distanceToPlayer <= enemyData.aggroRadius) StartChasing();
                    break;

                case AIState.Chasing:
                    _agent.SetDestination(_playerTransform.position);
                    if (distanceToPlayer > enemyData.evadeRadius) StartReturning();
                    break;

                case AIState.Returning:
                    if (Vector3.Distance(transform.position, _startPosition) <= _agent.stoppingDistance + 0.5f) ResetToIdle();
                    break;
            }
        }

        private void StartChasing()
        {
            currentState = AIState.Chasing;
            _agent.speed = enemyData.chaseSpeed;
            Debug.Log($" {enemyData.enemyName} начал погоню!");
        }

        private void StartReturning()
        {
            currentState = AIState.Returning;
            _agent.speed = enemyData.returnSpeed;
            _agent.SetDestination(_startPosition);
            Debug.Log($" {enemyData.enemyName} возвращается домой.");
        }

        private void ResetToIdle()
        {
            currentState = AIState.Idle;
            _agent.ResetPath();
        }

        private void OnDrawGizmosSelected()
        {
            if (enemyData == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.aggroRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.evadeRadius);
        }
    }
}