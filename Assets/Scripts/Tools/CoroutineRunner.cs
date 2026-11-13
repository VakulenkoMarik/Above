using System.Collections;
using DependencyControl;
using UnityEngine;

namespace Tools
{
    public class CoroutineRunner : DependencyMonoBehaviour
    {
        public Coroutine Run(IEnumerator coroutine) {
            return StartCoroutine(coroutine);
        }
        
        public Coroutine Stop(IEnumerator coroutine) {
            return StartCoroutine(coroutine);
        }

        public void StopAll() {
            StopAllCoroutines();
        }
    }
}