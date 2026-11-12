using ProjectCraft._Server;
using UnityEngine;
using System;

public class ServerTargetController : MonoBehaviour
{

    [Header("Current Target (Server Side)")]
    public Transform currentTarget;

    [Header("Targeting Settings")]
    [SerializeField] private float maxTargetDistance = 25f;

    public event Action<Transform> OnTargetChanged;
    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            ClearTarget();
            return;
        }

        var damageable = newTarget.GetComponentInParent<IDamageable>();
        if (damageable != null && damageable.Transform.CompareTag("Enemy"))
        {
            currentTarget = damageable.Transform;
            string targetName = (damageable is ServerEnemyEntity enemy) ? enemy.EntityName : currentTarget.name;
            Debug.Log($"Цель успешно захвачена: {targetName}");
            OnTargetChanged?.Invoke(currentTarget);
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
            OnTargetChanged?.Invoke(null);
            Debug.Log("Цель сброшена");
        }
        
    }



}
