using UnityEngine;

// TODO: implement spawn system
// TODO: refactor whole project with ECS
namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
        
            if (_currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log(other.gameObject.layer);
            if (other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            {
                TakeDamage(50);
                Destroy(other.gameObject);
            }
        }
    }
}
