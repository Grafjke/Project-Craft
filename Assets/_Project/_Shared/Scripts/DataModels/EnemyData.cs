using UnityEngine;

namespace ProjectCraft._Shared
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "GDD/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
       
        [Header("Meta Info")]
        public string enemyID;
        public string enemyName;

        [Header("Movement & AI Settings")]
        public float aggroRadius = 8f;
        public float evadeRadius = 15f;
        public float chaseSpeed = 4.5f;
        public float returnSpeed = 3.5f;

        [Header("Stats Blueprint (MVP)")]
        public int maxHealth = 100;
        public int baseDamage = 10;
    }
}
