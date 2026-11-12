using UnityEngine;

namespace ProjectCraft._Server
{
    public class ServerHomingProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 22f;
        [SerializeField] private float hitDistanceThreshold = 0.35f;

        private IDamageable _target;
        private DamageInfo _damageInfo;
        private bool _isInitialized;

        public void Initialize(IDamageable target, DamageInfo damageInfo)
        {
            _target = target;
            _damageInfo = damageInfo;
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized) return;

            if (_target == null || _target.IsDead)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 targetPos = _target.HitTargetPoint.position;
            Vector3 direction = (targetPos - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            transform.position += direction * (speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) <= hitDistanceThreshold)
            {
                _target.TakeDamage(_damageInfo);
                Destroy(gameObject);
            }
        }
    }
}