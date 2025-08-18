using System;
using UnityEngine;

namespace Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private bool disableColliderOnDeath = true;
        public int CurrentHp { get; private set; }

        public Action OnDamageTaken;
        public Action OnDeath;

        public void Initialize(int hpAmount) {
            CurrentHp = hpAmount;
        }
        
        public void TakeDamage(int amount) {
            CurrentHp -= amount;
            OnDamageTaken?.Invoke();
            
            if (CurrentHp <= 0) {
                Death();
            }
        }

        private void Death() {
            TryDisableColliderOnDeath();
            OnDeath?.Invoke();
        }
        
        private void TryDisableColliderOnDeath() {
            if (disableColliderOnDeath) {
                GetComponent<Collider>().enabled = false;
            }
        }
    }
}