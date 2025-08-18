using Components;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using Game;
using UnityEngine;
using UnityEngine.Events;

namespace Hero
{
    public enum HorizontalDirection
    {
        Right,
        Left,
    }
    
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(MoveComponent))]
    [RequireComponent(typeof(CoinsCollectorComponent))]
    public class Character : MonoBehaviour
    {
        [field: SerializeField] public int Index { get; private set; }
        
        [SerializeField] private UnityEvent onDeath;
        [SerializeField] private UnityEvent onJump;
        [SerializeField] private UnityEvent onCoinCollected;
        
        private HorizontalDirection _currentHorizontalDirection = HorizontalDirection.Right;

        public HealthComponent HealthComponent { get; private set; }
        public MoveComponent MoveComponent { get; private set; }
        public CoinsCollectorComponent CoinsCollectorComponent { get; private set; }

        private VisualEffectsPlayer VisualEffects => _visualEffects ??= Dependency.Get<VisualEffectsPlayer>();
        private VisualEffectsPlayer _visualEffects;
        
        private void Start() {
            InitializeComponents();
            SetUpProperties();
        }

        private void InitializeComponents() {
            AssignComponents();
            SetUpHealthEffects();
            SetUpMoveEffects();
            SetUpMoveCoinCollectionEffects();
        }

        private void AssignComponents() {
            MoveComponent = GetComponent<MoveComponent>();
            HealthComponent = GetComponent<HealthComponent>();
            CoinsCollectorComponent = GetComponent<CoinsCollectorComponent>();
        }

        private void SetUpHealthEffects() {
            HealthComponent.OnDeath += VisualEffects.TryVibrate;
            
            ShakingData onDeathShakeData = new ShakingData(gameObject.transform);
            VisualEffects.SetShakingData(onDeathShakeData);
            HealthComponent.OnDeath += VisualEffects.TryShake;
            
            HealthComponent.OnDeath += onDeath.Invoke;
        }

        private void SetUpMoveEffects() {
            MoveComponent.OnJump += onJump.Invoke;
        }
        
        private void SetUpMoveCoinCollectionEffects() {
            CoinsCollectorComponent.OnCoinCollected += onCoinCollected.Invoke;
        }
        
        private void SetUpProperties() {
            float speed = Dependency.Get<JsonStorage>().data.currentShop.currentBoost == 2 ? 1.5f : 2.2f;
            int hp = Dependency.Get<JsonStorage>().data.currentShop.currentBoost == 3 ? 2 : 1;
            float jumpForce = 7f;
            
            HealthComponent.Initialize(hp);
            MoveComponent.SetSpeed(speed);
            MoveComponent.SetJumpForce(jumpForce);
        }

        private void OnCollisionEnter(Collision other) {
            if (other.collider.tag.Equals("Wall")) {
                Bounce();
            }
        }

        private void Bounce() {
            Vector3 newDirection = GetDirection();
            MoveComponent.SetDirection(newDirection);
            
            ChangeHorizontalDirection();
        }
        
        private Vector3 GetDirection() {
            return _currentHorizontalDirection 
                   == HorizontalDirection.Right ? Vector3.right 
                    : Vector3.left;
        }

        private void ChangeHorizontalDirection() {
            if (_currentHorizontalDirection.Equals(HorizontalDirection.Right)) {
                _currentHorizontalDirection = HorizontalDirection.Left;
                return;
            }

            _currentHorizontalDirection = HorizontalDirection.Right;
        }

        private void OnDestroy() {
            HealthComponent.OnDeath -= VisualEffects.TryVibrate;
            HealthComponent.OnDeath -= VisualEffects.TryShake;
            HealthComponent.OnDeath -= onDeath.Invoke;
            
            MoveComponent.OnJump -= onJump.Invoke;
            
            CoinsCollectorComponent.OnCoinCollected -= onCoinCollected.Invoke;
        }
    }
}