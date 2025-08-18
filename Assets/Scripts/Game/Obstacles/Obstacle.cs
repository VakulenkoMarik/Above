using Components;
using UnityEngine;

namespace Game.Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private int damage;

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.TryGetComponent<HealthComponent>(out var health)) {
                health.TakeDamage(damage);
            }
        }
    }
}