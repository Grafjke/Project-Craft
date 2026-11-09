using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
public class ServerMovementController : MonoBehaviour
{

    private NavMeshAgent _agent;
    private bool _isStunned = false;


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
            Debug.Log($"[Movement] Игрок успешно телепортирован на точку спавна новой сцены: {scene.name}");
        }
        else
        {
            Debug.LogWarning($"[Movement] На сцене {scene.name} не найдена точка спавна с тегом 'SpawnPoint'!");
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

    
    public void OnClientMovementRequest(Vector3 targetPosition)
    {
      
        if (_isStunned) return;

        _agent.SetDestination(targetPosition);
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
