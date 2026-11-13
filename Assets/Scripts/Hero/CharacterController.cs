using DependencyControl;
using Game;
using UnityEngine;

namespace Hero
{
    public class CharacterController : MonoBehaviour
    {
        private Character _currentCharacter;

        private void Start() {
            _currentCharacter = Dependency.Get<RoundInitialization>().GetCurrentCharacter();
        }

        public void Jump() {
            _currentCharacter.MoveComponent.Jump();
        }

        public void SetSpeed(float speed) {
            _currentCharacter.MoveComponent.SetSpeed(speed);
        }
    }
}