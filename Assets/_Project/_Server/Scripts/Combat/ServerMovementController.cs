using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class ServerMovementController : MonoBehaviour
{

    private NavMeshAgent _agent;
    private bool _isStunned = false; 

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
