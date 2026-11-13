using System.Collections;
using DependencyControl;
using Firebase.DataControl;
using Firebase.Storage;
using UnityEngine;

namespace Game
{
    public struct ShakingData
    {
        public ShakingData(Transform target) {
            Target = target;
            Times = 10;
            Amplitude = 0.1f;
            Interval = 0.05f;
        }
        
        public Transform Target;
        
        public int Times;
        public float Amplitude;
        public float Interval;
    }
    
    public class VisualEffectsPlayer : DependencyMonoBehaviour
    {
        private JsonStorage Storage => _jsonStorage ??= Dependency.Get<JsonStorage>();
        private JsonStorage _jsonStorage;

        private ShakingData _lastShakingData;
        
        public void TryVibrate() {
            if (CanVibrate()) {
                Handheld.Vibrate();
            }
        }

        private bool CanVibrate() {
            return Storage.data.otherSettings.vibration;
        }
        
        public void TryShake() {
            TryShake(_lastShakingData);
        }
        
        public void TryShake(ShakingData shakingDataParams) {
            if (CanShake()) {
                SetShakingData(shakingDataParams);
                StartCoroutine(nameof(Shake));
            }
        }

        private bool CanShake() {
            return Storage.data.otherSettings.cameraShake;
        }

        public void SetShakingData(ShakingData shakingData) {
            _lastShakingData = shakingData;
        }
        
        public IEnumerator Shake() {
            Transform target = _lastShakingData.Target;
            Vector3 originalPosition = target.position;
            
            float amplitude = _lastShakingData.Amplitude;

            for (int i = 0; i < _lastShakingData.Times; i++) {
                float offset = i % 2 == 0 ? amplitude : -amplitude;
                target.position = new Vector3(originalPosition.x + offset, originalPosition.y, originalPosition.z);
                yield return new WaitForSeconds(_lastShakingData.Interval);
            }

            target.position = originalPosition;
        }
    }
}