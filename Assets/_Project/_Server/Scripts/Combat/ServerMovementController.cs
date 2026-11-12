using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class ServerMovementController : MonoBehaviour
{

    private NavMeshAgent _agent;
    private bool _isStunned = false;
    public bool IsMoving
    {
        get
        {
            if (_agent == null || !_agent.isOnNavMesh) return false;

            return _agent.pathPending ||
                   _agent.remainingDistance > _agent.stoppingDistance + 0.05f ||
                   _agent.velocity.sqrMagnitude > 0.01f;
        }
    }


    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");

        if (spawnPoint != null)
        {
            TeleportToPosition(spawnPoint.transform.position);
        }
        else
        {
            Debug.LogWarning($"[Movement] На сцене {scene.name} не найдена точка спавна с тегом 'SpawnPoint'!");
        }
    }

    public void OnClientMovementRequest(Vector3 targetPosition)
    {
        if (_isStunned) return;

        _agent.stoppingDistance = 0.1f;
        _agent.SetDestination(targetPosition);
    }
    public void MoveToTarget(Vector3 targetPosition, float stopDistance)
    {
        if (_isStunned || !_agent.isOnNavMesh) return;

        _agent.stoppingDistance = Mathf.Max(0.1f, stopDistance);
        _agent.SetDestination(targetPosition);
    }

    public void StopMovement()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
    }

    public void TeleportToPosition(Vector3 newPosition)
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();

        _agent.enabled = false;
        transform.position = newPosition;
        _agent.enabled = true;
    }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    
    public void SetStun(bool state)
    {
        _isStunned = state;
        if (_isStunned && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
        }
    }


}
