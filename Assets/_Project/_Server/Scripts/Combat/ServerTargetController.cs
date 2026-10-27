using UnityEngine;

public class ServerTargetController : MonoBehaviour
{

    [Header("Current Target (Server Side)")]
    public Transform currentTarget;

    [Header("Targeting Settings")]
    [SerializeField] private float maxTargetDistance = 25f;

    
    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            ClearTarget();
            return;
        }

        
        if (newTarget.CompareTag("Enemy"))
        {
            currentTarget = newTarget;
            Debug.Log($"Цель успешно захвачена: {currentTarget.name}");
            
        }
    }

    
    public void SelectTargetViaTab()
    {
      
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Debug.LogWarning("На сцене не найдено объектов с тегом Enemy!");
            return;
        }

        Transform bestTarget = null;
        float closestDistance = maxTargetDistance;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = enemy.transform;
            }
        }

       
        if (bestTarget != null)
        {
            SetTarget(bestTarget);
        }
    }

    public void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget = null;
            Debug.Log("Цель сброшена");
        }
    }



}
