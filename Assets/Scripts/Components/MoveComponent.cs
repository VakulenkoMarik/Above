using System;
using UnityEngine;

namespace Components
{
    public class MoveComponent : MonoBehaviour
    {
        public float JumpForce { get; private set; }
        public float Speed { get; private set; }
        public Vector3 Direction { get; private set; }

        public Action OnJump;
        public Action OnMove;
        
        private Rigidbody2D _rb;

        private void Start() {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void SetSpeed(float newSpeed) {
            Speed = newSpeed;
        }

        public void SetDirection(Vector3 newDirection) {
            Direction = newDirection;
        }
        
        public void SetJumpForce(float newJumpForce) {
            JumpForce = newJumpForce;
        }
        
        private void Update() {   
            TryMove();
        }

        private void TryMove() {
            if (CanMove()) {
                Move();
            }
        }
        
        private bool CanMove() {
            return Speed > 0 && Direction != Vector3.zero;
        }

        private void Move() {
            transform.position += Speed * Time.deltaTime * Direction;
            OnMove?.Invoke();
        }
        
        public void Jump() {
            ThrowUp();
            OnJump?.Invoke();
        }

        private void ThrowUp() {
            _rb.velocity = new Vector2(0, JumpForce);
        }
    }
}