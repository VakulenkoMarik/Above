using Audio;
using Firebase.DataControl;
using UnityEngine;

namespace Game
{
    public enum CoinType
    {
        FlyCoin,
        SuperCoin,
    }
    
    public class Coin : MonoBehaviour
    {
        [field: SerializeField] public CoinType Type { get; private set; }
        [field: SerializeField] public int Amount { get; private set; } = 1;

        [SerializeField] private string pickUpAnimationTrigger;
        [SerializeField] private AudioPlayer audioPlayer;

        private Animator _animator;
        private Collider _collider;

        private void Start() {
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
        }

        public void PickUp() {
            _animator.SetTrigger(pickUpAnimationTrigger);
            _collider.enabled = false;
            audioPlayer.PlayOneShot();
        }

        public void ApplyToUser(ref Data.UserData userData, int multiplier) {
            int coinsToAdd = Amount * multiplier;

            switch (Type)
            {
                case CoinType.FlyCoin:
                    userData.coinsF += coinsToAdd;
                    break;
                case CoinType.SuperCoin:
                    userData.coinsS += coinsToAdd;
                    break;
            }
        }
    }
}